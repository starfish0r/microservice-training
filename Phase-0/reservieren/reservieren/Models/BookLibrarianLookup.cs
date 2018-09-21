namespace reservieren.Models
{
    using System.Collections.Generic;

    using Akka.Actor;
    using Akka.Persistence.Query;
    using Akka.Persistence.Query.Sql;
    using Akka.Streams;

    public partial class BookLibrarianLookup : UntypedActor
    {
        public static Props props(SqlReadJournal readJournal)
            => Props.Create(() => new BookLibrarianLookup(readJournal));

        private readonly SqlReadJournal readJournal;

        public BookLibrarianLookup(SqlReadJournal journal)
        {
            readJournal = journal;
            readJournal.PersistenceIds().RunForeach(id => knownIds.Add(id), mat);
        }

        private readonly ActorMaterializer mat = ActorMaterializer.Create(Context);

        private readonly HashSet<string> knownIds = new HashSet<string>();

        protected override void OnReceive(object message)
        {
            switch (message)
            {
                case GetBook book:
                    if (knownIds.Contains(BookUtilities.CreateBookName(book.Isbn)))
                    {
                        var task = readJournal
                                .CurrentEventsByPersistenceId(BookUtilities.CreateBookName(book.Isbn), 0,
                                                              long.MaxValue)
                                .RunAggregate(new BookLookupResult(), ReadBook, mat);
                        task.Wait();
                        Sender.Tell(task.Result);
                    }
                    else
                    {
                        Sender.Tell(new BookNotFound());
                    }

                    break;
            }
        }

        private static BookLookupResult ReadBook(BookLookupResult bookResult, EventEnvelope envelope)
        {
            switch (envelope)
            {
                case EventEnvelope e when e.Event is Book.BookIsbnUpdated:
                    var isbnEvent = (Book.BookIsbnUpdated) e.Event;
                    bookResult.Isbn = isbnEvent.Isbn;
                    break;
                case EventEnvelope e when e.Event is Book.BookTitleUpdated:
                    var titleEvent = (Book.BookTitleUpdated) e.Event;
                    bookResult.Title = titleEvent.Title;
                    break;
                case EventEnvelope e when e.Event is Book.BookAuthorUpdated:
                    var authorEvent = (Book.BookAuthorUpdated) e.Event;
                    bookResult.Author = authorEvent.Author;
                    break;
                case EventEnvelope e when e.Event is Book.BookShortDescriptionUpdated:
                    var shortDescriptionEvent = (Book.BookShortDescriptionUpdated) e.Event;
                    bookResult.ShortDescription = shortDescriptionEvent.ShortDescription;
                    break;
                case EventEnvelope e when e.Event is Book.BookLentUpdated:
                    var lentEvent = (Book.BookLentUpdated) e.Event;
                    bookResult.Lent = lentEvent.Lent;
                    break;
                case EventEnvelope e when e.Event is Book.BookReservedUpdated:
                    var reservedEvent = (Book.BookReservedUpdated) e.Event;
                    bookResult.Reserved = reservedEvent.Reserved;
                    break;
            }

            return bookResult;
        }
    }
}
