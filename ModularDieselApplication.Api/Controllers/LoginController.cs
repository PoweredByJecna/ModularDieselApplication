using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models; // Např. vaše uživatelská entita
using System.Threading.Tasks;

namespace ModularDieselApplication.Api.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        private readonly SignInManager<TableUser> _signInManager;

        public LoginController(SignInManager<TableUser> signInManager)
        {
            _signInManager = signInManager;
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
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            // Kontrola validace formuláře
            if (!ModelState.IsValid)
            {
                // Pokud data nesplňují validační pravidla, vrátíme formulář s chybami
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Input.UserName,
                model.Input.Password,
                model.Input.RememberMe,
                lockoutOnFailure: false);

            if (!result.Succeeded)
            {
                // Pokud se přihlášení nezdařilo, přidáme chybovou hlášku
                ModelState.AddModelError(string.Empty, "Špatné uživatelské jméno nebo heslo.");
                return View(model);
            }

            // Přihlášení bylo úspěšné – přesměrujeme na výchozí stránku (např. Dieslovani/Index)
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