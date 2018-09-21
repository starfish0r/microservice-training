namespace reservieren.Services
{
    using Akka.Actor;

    /// <summary>
    /// Wrapper class to pass a dedicated actorRef into a controller,
    /// with the dotnet mvc dependency injection.
    /// </summary>
    public class BookLibrarianActorRef : IBookLibrarianActorRef
    {
        private readonly IActorRef BookLibrarianActorWrapper;

        public BookLibrarianActorRef(IActorRef bookLibrarian) => BookLibrarianActorWrapper = bookLibrarian;

        public IActorRef GetActorRef() => BookLibrarianActorWrapper;
    }
}
