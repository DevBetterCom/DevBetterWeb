using DevBetterWeb.Core.Events;
using MediatR;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DevBetterWeb.Web.Controllers;

public class HomeController : Controller
{
  private readonly IWebHostEnvironment _webHostEnvironment;
  private readonly ILogger<HomeController> _logger;
	private readonly IMediator _mediator;

	public HomeController(IWebHostEnvironment webHostEnvironment,
          ILogger<HomeController> logger,
					IMediator mediator)
  {
    _webHostEnvironment = webHostEnvironment;
    _logger = logger;
		_mediator = mediator;
	}

  public IActionResult Index()
  {
    ViewData.Add("env", _webHostEnvironment.EnvironmentName);
    return View();
  }

	[Route("/Home/StripeVerification")]
  public IActionResult StripeVerification()
  {
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
			_mediator.Publish(exceptionEvent);

      _logger.LogError(feature.Error, "devBetter global error caught");
    }
    return View();
  }
}
