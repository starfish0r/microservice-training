namespace reservieren.Controllers
{
    using System.Threading.Tasks;

    using Akka.Actor;

    using Eventing;

    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Net.Http.Headers;

    using Services;

    /// <inheritdoc />
    /// <summary>
    /// Provides the ActionResult for the controller.
    /// To keep the connection open until a client is closing the connection
    /// a task is returned which never ends.
    /// </summary>
    public class PushActorStreamResult : IActionResult
    {
        private readonly string _contentType;

        private readonly IEventConnectionHolderActorRef _connectionHolderActorRef;

        public PushActorStreamResult(IEventConnectionHolderActorRef pConnectionHolderActorRef, string pContentType)
        {
            _contentType = pContentType;
            _connectionHolderActorRef = pConnectionHolderActorRef;
        }

        public Task ExecuteResultAsync(ActionContext pContext)
        {
            var stream = pContext.HttpContext.Response.Body;
            pContext.HttpContext.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue(_contentType);

            return _connectionHolderActorRef.GetActorRef()
                                            .Ask(new ConnectionHolder.NewConnection(
                                                         stream, pContext.HttpContext.RequestAborted));
        }
    }
}
