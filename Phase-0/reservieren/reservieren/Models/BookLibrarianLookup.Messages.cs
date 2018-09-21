namespace reservieren.Models
{
    public partial class BookLibrarianLookup
    {
        public sealed class GetBook
        {
            public string Isbn { get; }

            public GetBook(string isbn) => Isbn = isbn;
        }
    }
}
