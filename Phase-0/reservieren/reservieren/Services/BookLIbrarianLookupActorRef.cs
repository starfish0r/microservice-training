namespace reservieren.Services
{
    using Akka.Actor;

    /// <summary>
    /// Wrapper class to pass a dedicated actorRef into a controller,
    /// with the dotnet mvc dependency injection.
    /// </summary>
    public class BookLIbrarianLookupActorRef : IBookLibrarianLookupActorRef
    {
        private readonly IActorRef BookLibrarianLookupActorWrapper;

        public BookLIbrarianLookupActorRef(IActorRef bookLibrarianLookup)
            => BookLibrarianLookupActorWrapper = bookLibrarianLookup;

        public IActorRef GetActorRef() => BookLibrarianLookupActorWrapper;
    }
}
