namespace reservieren.Eventing
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading;

    using Akka.Actor;
    using Akka.Event;
    using Akka.Persistence.Query;
    using Akka.Persistence.Query.Sql;
    using Akka.Streams;

    using Models;

    using Newtonsoft.Json;

    /// <inheritdoc />
    /// <summary>
    /// Writes received events from the persistent query stream as Server Sent Events to the response
    /// stream of the connection.
    /// </summary>
    public partial class ConnectionHandler : UntypedActor
    {
        public static Props props(Stream pStream, CancellationToken pCancellationToken)
            => Props.Create(() => new ConnectionHandler(pStream, pCancellationToken));

        private readonly CancellationToken _cancellationToken;

        private readonly SqlReadJournal _readJournal;

        private readonly ActorMaterializer _materializer;

        private readonly StreamWriter _writer;

        private ILoggingAdapter _log = Context.GetLogger();

        public ConnectionHandler(Stream pStream, CancellationToken pCancellationToken)
        {
            _cancellationToken = pCancellationToken;
            _readJournal = PersistenceQuery
                           .Get(Context.System).ReadJournalFor<SqlReadJournal>("akka.persistence.query.journal.sql");
            _materializer = ActorMaterializer.Create(Context.System);
            _writer = new StreamWriter(pStream)
                      {
                              NewLine = "\n"
                      };
        }

        protected override void PreStart()
        {
            base.PreStart();

            //Inside the event flow self isn't the actor to receive
            var connectionHandler = Self;

            _readJournal.EventsByTag("book-reserved-updated").RunForeach(
                    pEnvelope =>
                    {
                        switch (pEnvelope)
                        {
                            case EventEnvelope e when e.Event is Book.BookReservedUpdated:

                                long id = ((Sequence) e.Offset).Value;

                                var reservedEvent = (Book.BookReservedUpdated) e.Event;
                                string isbn = e.PersistenceId.Split("-").Last();

                                connectionHandler.Tell(
                                        new BookReservedEventEnvelope(
                                                id.ToString(),
                                                new BookReservedEvent(isbn, reservedEvent.Reserved)));

                                break;
                        }
                    }, ActorMaterializer.Create(Context.System));

            ValidateConnection();
            KeepAliveConnection();
        }

        protected override void OnReceive(object pMessage)
        {
            switch (pMessage)
            {
                case BookReservedEventEnvelope e:
                    if (!ConnectionIsClosed())
                    {
                        _writer.WriteLine($"id:{e.Id}");
                        _writer.WriteLine("event: book-reserved-updated");
                        _writer.WriteLine($"data: {JsonConvert.SerializeObject(e.BookReservedEvent)}");
                        _writer.WriteLine();
                        _writer.Flush();
                    }

                    break;
                case CheckConnection _:
                    if (!ConnectionIsClosed())
                    {
                        ValidateConnection();
                    }

                    break;
                case HeartBeat _:
                    if (!ConnectionIsClosed())
                    {
                        _writer.WriteLine(":heartbeat");
                        _writer.WriteLine();
                        _writer.Flush();

                        KeepAliveConnection();
                    }

                    break;
            }
        }

        private void ValidateConnection()
        {
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(5), Self, new CheckConnection(),
                                                      ActorRefs.NoSender);
        }

        private void KeepAliveConnection()
        {
            Context.System.Scheduler.ScheduleTellOnce(TimeSpan.FromSeconds(60), Self, new HeartBeat(),
                                                      ActorRefs.NoSender);
        }

        private bool ConnectionIsClosed()
        {
            bool isClosed = false;
            if (_cancellationToken.IsCancellationRequested)
            {
                _writer.Dispose();
                Self.Tell(PoisonPill.Instance);
                isClosed = true;
            }

            return isClosed;
        }
    }
}
