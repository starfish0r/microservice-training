namespace reservieren.Models
{
    public partial class BookLibrarianLookup
    {
        public sealed class BookLookupResult
        {
            public string Isbn { get; set; }

            public string Author { get; set; }

            public string Title { get; set; }

            public string ShortDescription { get; set; }

            public bool Reserved { get; set; }

            public bool Lent { get; set; }
        }

        public sealed class BookNotFound
        {
        }
    }
}
