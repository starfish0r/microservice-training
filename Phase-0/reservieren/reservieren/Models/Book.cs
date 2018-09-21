namespace reservieren.Models
{
    using Akka.Actor;
    using Akka.Persistence;

    public partial class Book : UntypedPersistentActor
    {
        private string Id;

        private string Isbn;

        private string Title;

        private string Author;

        private string ShortDescription;

        private bool Lent;

        private bool Reserved;

        public static Props props => Props.Create(() => new Book());

        //protected ILoggingAdapter Log { get; } = Context.GetLogger();

        public override string PersistenceId { get; } = Context.Self.Path.Name;

        protected override void PreStart()
        {
            base.PreStart();
            Log.Info($"Book: {Context.Self.Path.Name}");
        }

        protected override void OnCommand(object message)
        {
            switch (message)
            {
                case BookData book:

                    if (Id != book.Id)
                    {
                        Persist(new BookIdUpdated(book.Id), evt => Id = book.Id);
                    }

                    if (Isbn != book.Isbn)
                    {
                        Persist(new BookIsbnUpdated(book.Isbn), evt => Isbn = book.Isbn);
                    }

                    if (Title != book.Titel)
                    {
                        Persist(new BookTitleUpdated(book.Titel), evt => Title = book.Titel);
                    }

                    if (Author != book.Autor)
                    {
                        Persist(new BookAuthorUpdated(book.Autor), evt => Author = book.Autor);
                    }

                    if (ShortDescription != book.KurzBeschreibung)
                    {
                        Persist(new BookShortDescriptionUpdated(book.KurzBeschreibung),
                                evt => ShortDescription = book.KurzBeschreibung);
                    }
                    break;
                case BookLent bookLent:
                    if (Lent != bookLent.Lent)
                    {
                        Persist(new BookLentUpdated(bookLent.Lent), evt => Lent = bookLent.Lent);
                    }
                    if (Isbn != bookLent.Isbn)
                    {
                        Persist(new BookIsbnUpdated(bookLent.Isbn), evt => Isbn = bookLent.Isbn);
                    }
                    break;
                case Reserve _:
                    if (!Reserved)
                    {
                        Persist(new BookReservedUpdated(true), evt => Reserved = evt.Reserved);
                    }
                    Sender.Tell(new BookReserved(true));
                    break;
                case CancelReserve _:

                    if (Reserved)
                    {
                        Persist(new BookReservedUpdated(false), evt => Reserved = evt.Reserved);
                    }
                    Sender.Tell(new CanceledReserve(true));
                    break;
            }
        }

        protected override void OnRecover(object message)
        {
            switch (message)
            {
                case BookIdUpdated evt:
                    Id = evt.Id;
                    break;
                case BookIsbnUpdated evt:
                    Isbn = evt.Isbn;
                    break;
                case BookTitleUpdated evt:
                    Title = evt.Title;
                    break;
                case BookAuthorUpdated evt:
                    Author = evt.Author;
                    break;
                case BookShortDescriptionUpdated evt:
                    ShortDescription = evt.ShortDescription;
                    break;
                case BookLentUpdated evt:
                    Lent = evt.Lent;
                    break;
                case BookReservedUpdated evt:
                    Reserved = evt.Reserved;
                    break;
            }
        }
    }
}
