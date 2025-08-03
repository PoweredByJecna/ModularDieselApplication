using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models; // Např. vaše uživatelská entita
using System.Threading.Tasks;
using ModularDieselApplication.Application.Interfaces;

namespace ModularDieselApplication.Api.Controllers
{
    [AllowAnonymous]
    public class LoginController(SignInManager<TableUser> _signInManager, IAuthService _authService) : Controller
    {
        [HttpGet]public IActionResult Index() => View(new LoginViewModel());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", model);
            }

            var result = await _authService.LoginAsync(model.Input.UserName, model.Input.Password, model.Input.RememberMe);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Špatné uživatelské jméno nebo heslo.");
                return View("Index", model);
            }
            return RedirectToAction("Index", "Dieslovani");
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();
            return RedirectToAction("Index", "Dieslovani");
        }
    }
}