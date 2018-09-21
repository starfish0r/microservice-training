namespace reservieren.Controllers
{
    using System.Threading.Tasks;

    using Akka.Actor;

    using Microsoft.AspNetCore.Mvc;

    using Models;

    using Services;

    public class ReservierenController : Controller
    {
        private readonly IBookLibrarianActorRef _bookLibrarianActorWrapper;

        private readonly IBookLibrarianLookupActorRef _bookLibrarianLookupWrapper;

        private readonly IEventConnectionHolderActorRef _connectionHolderActorRef;

        public ReservierenController(
                IBookLibrarianActorRef pBookLibrarian,
                IBookLibrarianLookupActorRef pBookLibrarianLookup,
                IEventConnectionHolderActorRef pConnectionHolderActorRef
        )
        {
            _bookLibrarianActorWrapper = pBookLibrarian;
            _bookLibrarianLookupWrapper = pBookLibrarianLookup;
            _connectionHolderActorRef = pConnectionHolderActorRef;
        }

        [HttpGet]
        public IActionResult Index() => View();

        [HttpGet("book/{isbn}")]
        public async Task<IActionResult> Book(string isbn)
        {
            
            SetCacheHeaders();
            
            var result = await FetchBookData(new BookLibrarianLookup.GetBook(isbn));
            
            switch (result)
            {
                case BookLibrarianLookup.BookLookupResult book:
                    ViewData["Book"] = book;
                    return View("Book");
                case BookLibrarianLookup.BookNotFound _:
                    ViewData["Isbn"] = isbn;
                    return View("BookNotFound");
                default:
                    return View("Error");
            }
        }

        [HttpPost("book/{isbn}/reserve")]
        public async Task<IActionResult> Reserve(string isbn, bool reserve)
        {
            SetCacheHeaders();
            
            var result = await ReserveBook(new BookLibrarian.ReserveBook(isbn, reserve));
                        
            switch (result)
            {
                case Book.BookReserved reserved:
                    ViewData["Isbn"] = isbn;
                    ViewData["Reserved"] = reserved.Success;
                    return View("Reserved");
                case Book.CanceledReserve canceled:
                    ViewData["Isbn"] = isbn;
                    ViewData["Canceled"] = canceled.Success;
                    return View("Canceled");
                default:
                    return View("Error");
            }
        }

        [HttpGet("book/{isbn}/reserve")]
        public async Task<IActionResult> IsReserved(string isbn)
        {

            SetCacheHeaders();
            
            var result = await FetchBookData(new BookLibrarianLookup.GetBook(isbn));

            switch (result)
            {
                case BookLibrarianLookup.BookLookupResult book:
                    ViewData["Isbn"] = isbn;
                    ViewData["Reserved"] = book.Reserved;
                    return View("IsReserved");
                case BookLibrarianLookup.BookNotFound _:
                    ViewData["Isbn"] = isbn;
                    return View("BookNotFound");
                default:
                    return View("Error");
            }
        }
        
        [HttpGet("events")]
        public IActionResult Events() => new PushActorStreamResult(_connectionHolderActorRef, "text/event-stream");

        private Task<object> ReserveBook(BookLibrarian.ReserveBook pMessage)
            => _bookLibrarianActorWrapper.GetActorRef().Ask<object>(pMessage);

        private Task<object> FetchBookData(BookLibrarianLookup.GetBook pMessage)
            => _bookLibrarianLookupWrapper.GetActorRef().Ask<object>(pMessage);

        private void SetCacheHeaders()
        {
            HttpContext.Response.Headers.Add("Cache-Control","no-cache, no-store, max-age=0, must-revalidate");
            HttpContext.Response.Headers.Add("Pragma", "no-cache");
            HttpContext.Response.Headers.Add("Expires", "Fri, 01 Jan 1990 00:00:00 GMT");
        }
        
    }
}
