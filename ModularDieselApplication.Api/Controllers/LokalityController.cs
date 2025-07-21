using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;

namespace ModularDieselApplication.Api.Controllers
{
    public class LokalityController : Controller
    {   
        private readonly ILokalityService _lokalityService;

        public LokalityController(ILokalityService lokalityService)
        {
            _lokalityService = lokalityService;
        }

        // ----------------------------------------
        // Render the main view for Lokality.
        // ----------------------------------------
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }

        // ----------------------------------------
        // Fetch Lokality data for a table.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            var lokality = await _lokalityService.GetAllLokalityAsync();
            return Json(new { data = lokality });
        }

        // ----------------------------------------
        // Fetch Lokality details as JSON.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> DetailLokalityJson(string Nazev)
        {
            var lokality = await _lokalityService.DetailLokalityJsonAsync(Nazev);
            return Json(new { data = lokality });
        }

        // ----------------------------------------
        // Render the detail view for a specific Lokality.
        // ----------------------------------------
        public async Task<IActionResult> DetailLokality(string nazev)
        {
            var lokality = await _lokalityService.DetailLokalityAsync(nazev);
            return View(lokality);
        }

        // ----------------------------------------
        // Fetch diesel-related data for a specific Lokality.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetDieslovaniNaLokalite(string nazev)
        {
            var dieslovani = await _lokalityService.GetDieslovaniNaLokaliteAsync(nazev);
            return Json(new { data = dieslovani });
        }

        // ----------------------------------------
        // Fetch outage-related data for a specific Lokality.
        // ----------------------------------------
        public async Task<IActionResult> GetOdstavkynaLokalite(string nazev)
        {
            var odstavky = await _lokalityService.GetOdstavkynaLokaliteAsync(nazev);
            return Json(new { data = odstavky });
        }
    }
}