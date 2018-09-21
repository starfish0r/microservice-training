namespace reservieren.Models
{
    public partial class Book
    {
        /// <summary>
        /// Model for Server Sent Events from einbuchen service.
        /// </summary>
        public class BookData
        {
            public string Id { get; set; }

            public string Isbn { get; set; }

            public string Titel { get; set; }

            public string Autor { get; set; }

            public string KurzBeschreibung { get; set; }
        }
    }
}
