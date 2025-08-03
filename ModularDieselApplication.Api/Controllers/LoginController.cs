using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Api.Controllers
{
    [AllowAnonymous]
    public class LoginController(IAuthService _authService) : Controller
    {
        [HttpGet] public IActionResult Index() => View(new LoginViewModel());

        [AllowAnonymous]
        [HttpPost]
        [Route("api/Login")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> LoginApi([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResult(new HandleResult(false, "Neplatný model."));
            }

            var result = await _authService.LoginAsync(model.Input.UserName, model.Input.Password, model.Input.RememberMe);

            if (!result.Succeeded) return JsonResult(new HandleResult(false, "špatné heslo nebo username"));
            return JsonResult(new HandleResult(true, "Úspěně přihlášeno", Url.Action("Index", "Dieslovani")));

        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Dieslovani");
        }
        private JsonResult JsonResult(HandleResult result)
        {
            return Json (new
            {
                succes = result.Success,
                message = result.Message,
                redirectURL = result.RedirectUrl,
            });
            
        }
        
        
    }
}