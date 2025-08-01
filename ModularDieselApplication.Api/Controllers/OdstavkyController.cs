
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Api.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly IOdstavkyService _odstavkyService;
        private readonly IServiceBaseClass _serviceBaseClass;

        public OdstavkyController(IOdstavkyService odstavkyService, IServiceBaseClass serviceBaseClass)
        {
            _odstavkyService = odstavkyService;
            _serviceBaseClass = serviceBaseClass;
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
            
            var result = await _odstavkyService.CreateNewOdstavka(lokalita, od, DO, popis);
            return JsonResult(result);

        }
        // ----------------------------------------
        // Delete an Odstavka by ID.
        // ---------------------------------------- 
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Odstavka,ActionFilter.Delete, id);
            return JsonResult(result);
        }
        // ----------------------------------------
        // Fetch Odstavky data for a table.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> GetTableData()
        {
            var data = await _serviceBaseClass.GetTableData(ServiceFilterEnum.Odstavka, DieslovaniOdstavkaFilterEnum.OD);
            return Json(new { data });
        }
        // ----------------------------------------
        // Change the time of an Odstavka.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> ChangeTimeOdstavky(string odstavkaId, DateTime time, string type)
        {
            var result = await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Odstavka,ActionFilter.ChangeTimeZactek, odstavkaId, time);
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