
using System.Collections.ObjectModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;

namespace ModularDieselApplication.Api.Controllers
{
    public class LokalityController : Controller
    {   
        private readonly ILokalityService  _lokalityService;
        public LokalityController(ILokalityService lokalityService)
        {
            _lokalityService=lokalityService;
        }

        [Authorize]
        public  IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            var lokality = await _lokalityService.GetAllLokalityAsync();   

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(), // Unikátní ID požadavku
                data = lokality
            });
        }
        [HttpGet]
        public async Task<IActionResult> DetailLokalityJson(int id)
        {
            var lokality = await _lokalityService.DetailLokalityJsonAsync(id);
            return Json(
            new
            {
                data=lokality
            });
        }
        public async Task<IActionResult> DetailLokality(int id)
        {
            var lokality = await _lokalityService.DetailLokalityAsync(id);
            if(lokality==null)
            {
                return NotFound();
            }
            return View(lokality);
          
        }
    
    }
}