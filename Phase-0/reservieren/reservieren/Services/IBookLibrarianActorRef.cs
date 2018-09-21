namespace reservieren.Services
{
    using Akka.Actor;

    public interface IBookLibrarianActorRef
    {
        IActorRef GetActorRef();
    }
}
