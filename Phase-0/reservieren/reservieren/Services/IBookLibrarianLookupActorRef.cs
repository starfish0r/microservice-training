namespace reservieren.Services
{
    using Akka.Actor;

    public interface IBookLibrarianLookupActorRef
    {
        IActorRef GetActorRef();
    }
}
