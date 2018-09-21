namespace reservieren.Models
{
    /// <inheritdoc />
    /// <summary>
    /// This partial class contains all the events that <see cref="T:reservieren.Models.Book" /> can react to.
    /// </summary>
    public partial class Book
    {
        public sealed class BookIdUpdated
        {
            public string Id { get; }

            public BookIdUpdated(string id) => Id = id;
        }

        public sealed class BookIsbnUpdated
        {
            public string Isbn { get; }

            public BookIsbnUpdated(string isbn) => Isbn = isbn;
        }

        public sealed class BookTitleUpdated
        {
            public string Title { get; }

            public BookTitleUpdated(string title) => Title = title;
        }

        public sealed class BookAuthorUpdated
        {
            public string Author { get; }

            public BookAuthorUpdated(string author) => Author = author;
        }

        public sealed class BookShortDescriptionUpdated
        {
            public string ShortDescription { get; }

            public BookShortDescriptionUpdated(string shortDescription) => ShortDescription = shortDescription;
        }

        public sealed class BookLentUpdated
        {
            public bool Lent { get; }

            public BookLentUpdated(bool lent) => Lent = lent;
        }

        public sealed class BookReservedUpdated
        {
            public bool Reserved { get; }

            public BookReservedUpdated(bool reserved) => Reserved = reserved;
        }
    }
}
