namespace reservieren.Eventing
{
    using System.IO;
    using System.Threading;

    public partial class ConnectionHolder
    {
        public sealed class NewConnection
        {
            public readonly Stream Stream;

            public readonly CancellationToken CancellationToken;

            public NewConnection(Stream pStream, CancellationToken pCancellationToken)
            {
                Stream = pStream;
                CancellationToken = pCancellationToken;
            }
        }
    }
}
