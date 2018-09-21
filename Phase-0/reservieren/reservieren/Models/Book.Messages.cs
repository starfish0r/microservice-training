namespace reservieren.Models
{
    /// <inheritdoc />
    /// <summary>
    /// This class contains all the messages that <see cref="T:reservieren.Models.Book" /> can send.
    /// </summary>
    public partial class Book
    {
        public sealed class BookLent
        {
            public bool Lent { get; }

            public string Isbn { get; }

            public BookLent(bool lent, string isbn)
            {
                Lent = lent;
                Isbn = isbn;
            }
        }

        public sealed class GetBookData
        {
        }

        public sealed class Reserve
        {
        }

        public sealed class CancelReserve
        {
        }

        public sealed class BookReserved
        {
            public bool Success { get; }

            public BookReserved(bool success) => Success = success;
        }

        public sealed class CanceledReserve
        {
            public bool Success { get; }

            public CanceledReserve(bool success) => Success = success;
        }
    }
}
