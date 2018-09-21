namespace reservieren.Models
{
    public partial class BookLibrarian
    {
        public class BookLentData
        {
            public string Isbn { get; }

            public bool Lent { get; }

            public BookLentData(string isbn, bool lent)
            {
                Isbn = isbn;
                Lent = lent;
            }
        }
    }
}
