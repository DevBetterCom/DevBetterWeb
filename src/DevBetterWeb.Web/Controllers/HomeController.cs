using DevBetterWeb.Core.Events;
using DevBetterWeb.Core.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Controllers
{
  public class HomeController : Controller
  {
    private readonly IWebHostEnvironment _webHostEnvironment;
    private readonly IDomainEventDispatcher _dispatcher;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IWebHostEnvironment webHostEnvironment,
            IDomainEventDispatcher dispatcher,
            ILogger<HomeController> logger)
    {
      _webHostEnvironment = webHostEnvironment;
      _dispatcher = dispatcher;
      _logger = logger;
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

        _logger.LogError(feature.Error, "devBetter global error caught");
      }
      return View();
    }
  }
}
