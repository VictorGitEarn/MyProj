using Microsoft.AspNetCore.Mvc;
using MP.Core.Communication;
using System.Linq;

namespace MP.WebApp.Controllers
{
    public class MainController : Controller
    {
        protected bool ResponseHasErros(ResponseResult resposta)
        {
            if (resposta != null && resposta.Errors.Mensagens.Any())
            {
                foreach (var mensagem in resposta.Errors.Mensagens)
                {
                    ModelState.AddModelError(string.Empty, mensagem);
                }

                return true;
            }

            return false;
        }
    }
}
