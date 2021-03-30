using Microsoft.AspNetCore.Mvc;

namespace MP.WebApp.Controllers
{
    public class ExternalDataController : MainController
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
