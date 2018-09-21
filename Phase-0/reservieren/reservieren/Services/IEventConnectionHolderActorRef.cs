namespace reservieren.Services
{
    using Akka.Actor;

    public interface IEventConnectionHolderActorRef
    {
        IActorRef GetActorRef();
    }
}
