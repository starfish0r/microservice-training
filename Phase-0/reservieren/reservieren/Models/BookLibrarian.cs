namespace reservieren.Models
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Threading.Tasks;

    using Akka.Actor;
    using Akka.Persistence;

    using EvtSource;

    using Newtonsoft.Json;

    public partial class BookLibrarian : UntypedPersistentActor
    {
        public static Props props => Props.Create(() => new BookLibrarian());

        public override string PersistenceId => Context.Self.Path.Name;

        private readonly HashSet<string> knownBooks = new HashSet<string>();

        private string booksLastMessageId = string.Empty;

        private string bookLentsLastMessageId = string.Empty;

        protected override void PreStart()
        {
            base.PreStart();
            Log.Info($"Start: {Context.Self.Path.Name}");
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case EventSourceMessageEventArgs messageCmd when messageCmd.Event == "BuchEingebucht":

                    Persist(new BookMessageReceived(messageCmd.Id),
                            bookMessage =>
                            {
                                var bookData = JsonConvert.DeserializeObject<Book.BookData>(messageCmd.Message);
                                bookData.Id = messageCmd.Id;
                                Persist(new BookIsbnReceived(bookData.Isbn),
                                        bookIsbnMessage =>
                                        {
                                            knownBooks.Add(bookIsbnMessage.Isbn);

                                            FindChild(bookData.Isbn,
                                                      bookActor => { bookActor.Tell(bookData); });
                                        });
                            });
                    break;
                case EventSourceMessageEventArgs messageCmd when messageCmd.Event == "book-lent-updated":
                    Persist(new BookLentMessageReceived(messageCmd.Id),
                            bookLentMessage =>
                            {
                                var bookLentData = JsonConvert.DeserializeObject<BookLentData>(messageCmd.Message);

                                /*
                                 * The messages are retrieved in a non guaranteed order. So everything is
                                 * set as it is available in this moment.
                                 */
                                if (!knownBooks.Contains(bookLentData.Isbn))
                                {
                                    Persist(
                                            new BookIsbnReceived(bookLentData.Isbn),
                                            bookIsbnMessage => { knownBooks.Add(bookLentData.Isbn); });
                                }

                                FindChild(bookLentData.Isbn, bookActor =>
                                                             {
                                                                 bookActor.Tell(
                                                                         new Book.BookLent(bookLentData.Lent,
                                                                                           bookLentData.Isbn));
                                                             });
                            });

                    break;
                case ReserveBook book:

                    FindChild(book.Isbn, bookActor =>
                                         {
                                             if (book.Reserve)
                                             {
                                                 bookActor.Forward(new Book.Reserve());
                                             }
                                             else
                                             {
                                                 bookActor.Forward(new Book.CancelReserve());
                                             }
                                         });
                    break;
                case object _:
                    Log.Info("unhandled-message:" + message);
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case BookMessageReceived evt:
                    booksLastMessageId = evt.Id;
                    break;
                case BookIsbnReceived evt:
                    knownBooks.Add(evt.Isbn);
                    break;
                case BookLentMessageReceived evt:
                    bookLentsLastMessageId = evt.Id;
                    break;
                case RecoveryCompleted _:
                    StartSSEListeningBooks(Self);
                    StartSSEListeningBooksLent(Self);
                    break;
            }
        }

        private void StartSSEListeningBooks(IActorRef actor)
        {
            Log.Info(
                    $"Read Events From: {Context.System.Settings.Config.GetString("container.bootcamp.einbuchen.url")}");
            var evr = new EventSourceReader(
                    new Uri(Context.System.Settings.Config.GetString("container.bootcamp.einbuchen.url")));

            /*
             * open the object via reflection otherwise the last event id could not.
             * This is important to read only new events and not all on scs restart.
             */
            var prop = evr.GetType().GetField("LastEventId", BindingFlags.NonPublic | BindingFlags.Instance);
            prop.SetValue(evr, booksLastMessageId);
            evr.Start();

            evr.MessageReceived += (sender, e) => actor.Tell(e);
            evr.Disconnected += async (sender, e) =>
                                {
                                    await Task.Delay(e.ReconnectDelay);

                                    // Reconnect to the same URL
                                    evr.Start();
                                };
        }

        private void StartSSEListeningBooksLent(IActorRef actor)
        {
            Log.Info(
                    $"Read Events From: {Context.System.Settings.Config.GetString("container.bootcamp.ausleihen.url")}");
            var evr = new EventSourceReader(
                    new Uri(Context.System.Settings.Config.GetString("container.bootcamp.ausleihen.url")));

            /*
             * open the object via reflection otherwise the last event id could not.
             * This is important to read only new events and not all on scs restart.
             */
            var prop = evr.GetType().GetField("LastEventId", BindingFlags.NonPublic | BindingFlags.Instance);

            prop.SetValue(evr, bookLentsLastMessageId);
            evr.Start();

            evr.MessageReceived += (sender, e) => actor.Tell(e);
            evr.Disconnected += async (sender, e) =>
                                {
                                    await Task.Delay(e.ReconnectDelay);
                                    evr.Start(); // Reconnect to the same URL
                                };
        }

        private static IActorRef CreateBook(string isbn)
            => Context.ActorOf(Book.props, BookUtilities.CreateBookName(isbn));

        private static void FindChild(string isbn, Action<IActorRef> f)
        {
            var book = Context.Child(BookUtilities.CreateBookName(isbn));
            if (book.IsNobody())
            {
                book = CreateBook(isbn);
            }

            f(book);
        }
    }
}
