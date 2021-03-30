using Microsoft.AspNetCore.Mvc;
using MP.WebApp.Models;
using MP.WebApp.Services;
using System.Threading.Tasks;

namespace MP.WebApp.Controllers
{
    public class UserController : MainController
    {
        private readonly IAuthService _authService;

        public UserController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpGet]
        [Route("new-account")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Route("new-account")]
        public async Task<IActionResult> Registro(RegisterUser registerUser)
        {
            if (!ModelState.IsValid) return View(registerUser);

            var resposta = await _authService.Register(registerUser);

            if (ResponseHasErros(resposta.ResponseResult)) return View(registerUser);

            await _authService.ToLogin(resposta);

            return RedirectToAction("Index", "ExternalData");
        }

        [HttpGet]
        [Route("login")]
        public IActionResult Login(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(LoginUser loginUser, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid) return View(loginUser);

            var resposta = await _authService.Login(loginUser);

            if (ResponseHasErros(resposta.ResponseResult)) return View(loginUser);

            await _authService.ToLogin(resposta);

            if (string.IsNullOrEmpty(returnUrl)) return RedirectToAction("Index", "ExternalData");

            return LocalRedirect(returnUrl);
        }

        [HttpGet]
        [Route("sair")]
        public async Task<IActionResult> Logout()
        {
            await _authService.Logout();

            return RedirectToAction("Index", "ExternalData");
        }
    }
}
