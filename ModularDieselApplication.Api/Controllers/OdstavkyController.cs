
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;

using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Api.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly IOdstavkyService _odstavkyService;

        public OdstavkyController(IOdstavkyService odstavkyService)
        {
            _odstavkyService = odstavkyService;
        }
        // ----------------------------------------
        // Render the main view for Odstavky.
        // ----------------------------------------
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        // ----------------------------------------
        // Suggest Lokality based on a query.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> SuggestLokalita(string query)
        {
            var lokalities = await _odstavkyService.SuggestLokalitaAsync(query);
            return Json(new { data = lokalities });
        }
        // ----------------------------------------
        // Create a new Odstavka.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Create(string lokalita, DateTime od, DateTime DO, string popis, string daOption)
        {
            
            var result = await _odstavkyService.CreateOdstavkaAsync(lokalita, od, DO, popis, daOption);
            return JsonResult(result);

        }
        // ----------------------------------------
        // Delete an Odstavka by ID.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _odstavkyService.DeleteOdstavkaAsync(id);
            return JsonResult(result);
d        }
        // ----------------------------------------
        // Fetch Odstavky data for a table.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> GetTableData()
        {
            var data = await _odstavkyService.GetTableDataAsync();
            return Json(new { data });
        }
        // ----------------------------------------
        // Change the time of an Odstavka.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> ChangeTimeOdstavky(string odstavkaId, DateTime time, string type)
        {
            var result = await _odstavkyService.ChangeTimeOdstavkyAsync(odstavkaId, time, type);
            return JsonResult(result);
        }
        private JsonResult JsonResult(HandleResult result)
        {
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
        }
    }
}