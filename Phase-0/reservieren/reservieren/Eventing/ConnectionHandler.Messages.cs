namespace reservieren.Eventing
{
    public partial class ConnectionHandler
    {
        private sealed class BookReservedEventEnvelope
        {
            
            public readonly string Id;

            public readonly BookReservedEvent BookReservedEvent;
            
            public BookReservedEventEnvelope(string pId, BookReservedEvent pBookReservedEvent)
            {
                Id = pId;
                BookReservedEvent = pBookReservedEvent;
            }
        }

        private sealed class CheckConnection
        {
        }

        private sealed class HeartBeat
        {
        }
        
    }
}
