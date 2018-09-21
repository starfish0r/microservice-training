namespace reservieren.Models
{
    public static class BookUtilities
    {
        private const string NAME_PREFIX = "book-isbn";

        public static string CreateBookName(string isbn) => $"{NAME_PREFIX}-{isbn}";
    }
}
