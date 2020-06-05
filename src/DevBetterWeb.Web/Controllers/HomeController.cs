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
        private readonly IExceptionHandlerPathFeature _exceptionHandlerPathFeature;
        private readonly IDomainEventDispatcher _dispatcher;

        public HomeController(IWebHostEnvironment webHostEnvironment,
            IExceptionHandlerPathFeature exceptionHandlerPathFeature,
            IDomainEventDispatcher dispatcher)
        {
            _webHostEnvironment = webHostEnvironment;
            _exceptionHandlerPathFeature = exceptionHandlerPathFeature;
            _dispatcher = dispatcher;
        }

        public IActionResult Index()
        {
            ViewData.Add("env", _webHostEnvironment.EnvironmentName);
            return View();
        }

        public IActionResult Error()
        {
            if(_exceptionHandlerPathFeature != null)
            {
                var exceptionEvent = new SiteErrorOccurredEvent(_exceptionHandlerPathFeature.Error);
                _dispatcher.Dispatch(exceptionEvent);
            }
            return View();
        }
    }
}
