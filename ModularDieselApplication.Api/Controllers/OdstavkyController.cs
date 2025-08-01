
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Api.Controllers
{
    public class OdstavkyController(IOdstavkyService _odstavkyService) : Controller
    {
        [Authorize]public IActionResult Index() => View();
        [HttpGet]public async Task<IActionResult> SuggestLokalita(string query)=>Json(await _odstavkyService.SuggestLokalitaAsync(query));
        [HttpPost]public async Task<IActionResult> Create(string lokalita, DateTime od, DateTime DO, string popis, string daOption)=>JsonResult(await _odstavkyService.CreateNewOdstavka(lokalita, od, DO, popis));
        [HttpPost]public async Task<IActionResult> Delete(string id)=>JsonResult(await _odstavkyService.ActionMethods(ServiceFilterEnum.Odstavka,ActionFilter.Delete, id));
        [HttpPost]public async Task<IActionResult> GetTableData()=>Json(await _odstavkyService.GetTableData());
        [HttpPost]public async Task<IActionResult> ChangeTimeOdstavky(string odstavkaId, DateTime time, string type)=>JsonResult(await _odstavkyService.ActionMethods(ServiceFilterEnum.Odstavka,ActionFilter.ChangeTimeZactek, odstavkaId, time));
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