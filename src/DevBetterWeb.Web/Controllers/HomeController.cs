using Microsoft.AspNetCore.Mvc;

namespace DevBetterWeb.Web.Controllers
{
    // TODO: Get rid of these in favor or pure razor pages?
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Error()
        {
            return View();
        }
    }
}
