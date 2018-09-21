namespace reservieren.Models
{
    public partial class BookLibrarian
    {
        public sealed class ReserveBook
        {
            public string Isbn { get; }

            public bool Reserve { get; }

            public ReserveBook(string isbn, bool reserve)
            {
                Isbn = isbn;
                Reserve = reserve;
            }
        }
    }
}
