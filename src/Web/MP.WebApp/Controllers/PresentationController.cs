using Microsoft.AspNetCore.Mvc;

namespace MP.WebApp.Controllers
{
    public class PresentationController : MainController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
