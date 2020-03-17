using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public HomeController(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            ViewData.Add("env", _webHostEnvironment.EnvironmentName);
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
