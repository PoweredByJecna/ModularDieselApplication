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
    public class LoginController : Controller
    {
        private readonly SignInManager<TableUser> _signInManager;
        private readonly IAuthService _authService;

        public LoginController(SignInManager<TableUser> signInManager,  IAuthService authService)
        {
            _signInManager = signInManager;
            _authService = authService;
        }
    
        // ----------------------------------------
        // GET: /Login/Index
        // ----------------------------------------
        [HttpGet]
        public IActionResult Index()
        {
            // Zobrazíme view a předáme nový model
            return View(new LoginViewModel());
        }

        // ----------------------------------------
        // POST: /Login/Index
        // ----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

                var result = await _authService.LoginAsync(model.Input.UserName, model.Input.Password, model.Input.RememberMe);

            if (!result.Succeeded)
            {
                ModelState.AddModelError(string.Empty, "Špatné uživatelské jméno nebo heslo.");
                return View(model);
            }

            return RedirectToAction("Index", "Dieslovani");
        }
        // ----------------------------------------
        // POST: /Login/Logout
        // ----------------------------------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Dieslovani");
        }
    }
}