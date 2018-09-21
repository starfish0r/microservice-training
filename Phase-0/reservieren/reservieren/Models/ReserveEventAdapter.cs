namespace reservieren.Models
{
    using System.Collections.Immutable;

    using Akka.Persistence.Journal;

    public class ReserveEventAdapter : IWriteEventAdapter
    {
        public string Manifest(object evt) => string.Empty;

        private static Tagged WithTag(object evt, string tag) => new Tagged(evt, ImmutableHashSet.Create(tag));

        public object ToJournal(object evt)
        {
            switch (evt)
            {
                case Book.BookReservedUpdated o:
                    return WithTag(o, "book-reserved-updated");
                default:
                    return evt;
            }
        }
    }
}
