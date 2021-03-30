using Microsoft.AspNetCore.Mvc;
using MP.Core.Communication;
using System.Linq;

namespace MP.WebApp.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponseHasErros(ResponseResult response)
        {
            if (response != null && response.Errors.Messages.Any())
            {
                foreach (var message in response.Errors.Messages)
                {
                    ModelState.AddModelError(string.Empty, message);
                }

                return true;
            }

            return false;
        }
    }
}