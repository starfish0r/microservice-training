namespace reservieren.Models
{
    /// <inheritdoc />
    public partial class BookLibrarian
    {
        /// <summary>
        /// Persists the last received books message id.
        /// </summary>
        public class BookMessageReceived
        {
            public string Id { get; }

            public BookMessageReceived(string id) => Id = id;
        }

        /// <summary>
        /// Persists the isbn for knowning our books.
        /// </summary>
        public class BookIsbnReceived
        {
            public string Isbn { get; }

            public BookIsbnReceived(string isbn) => Isbn = isbn;
        }

        /// <summary>
        /// Persists the last received books lent message id.
        /// </summary>
        public class BookLentMessageReceived
        {
            public string Id { get; }

            public BookLentMessageReceived(string id) => Id = id;
        }
    }
}
