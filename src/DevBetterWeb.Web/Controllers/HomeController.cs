using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IDomainEventDispatcher _dispatcher;

        public HomeController(IWebHostEnvironment webHostEnvironment,
            IDomainEventDispatcher dispatcher)
        {
            _webHostEnvironment = webHostEnvironment;
            _dispatcher = dispatcher;
        }

        public IActionResult Index()
        {
            ViewData.Add("env", _webHostEnvironment.EnvironmentName);
            return View();
        }

        public IActionResult Error()
        {
            var feature = HttpContext
              .Features
              .Get<IExceptionHandlerPathFeature>();
            if (feature != null)
            {
                var exceptionEvent = new SiteErrorOccurredEvent(feature.Error);
                _dispatcher.Dispatch(exceptionEvent);
            }
            return View();
        }
    }
}
