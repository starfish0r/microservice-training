namespace reservieren.Eventing
{

    using Akka.Actor;
    /// <inheritdoc />
    /// <summary>
    /// Manages the connection handlers.
    /// Spawn for every connection a handler actor.
    /// </summary>
    public partial class ConnectionHolder : UntypedActor
    {
        public static Props props() => Props.Create(() => new ConnectionHolder());
        
        protected override void OnReceive(object pMessage)
        {
            switch (pMessage)
            {
                case NewConnection c:
                    Context.ActorOf(ConnectionHandler.props(c.Stream, c.CancellationToken));
                    break;
            }
        }
    }
}
