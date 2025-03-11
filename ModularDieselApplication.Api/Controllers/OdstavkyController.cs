using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Api.Controllers
{
    public class OdstavkyController : Controller
    {
        private readonly IOdstavkyService _odstavkyService;

        public OdstavkyController(IOdstavkyService odstavkyService)
        {
            _odstavkyService = odstavkyService;
        }

        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> SuggestLokalita(string query)
        {
            var lokalities = await _odstavkyService.SuggestLokalitaAsync(query);
            return Json(lokalities);
        }
        [HttpPost]
        public async Task<IActionResult> Create(string lokalita, DateTime od, DateTime DO, string popis, string daOption)
        {
            var result = await _odstavkyService.CreateOdstavkaAsync(lokalita, od, DO, popis, daOption);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = result.Message,
                    odstavkaId = result.Odstavka?.ID,
                    dieslovaniId = result.Dieslovani?.ID
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Test()
        {
            var result = await _odstavkyService.TestOdstavkaAsync();
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new
                {
                    success = true,
                    message = result.Message,
                    odstavkaId = result.Odstavka?.ID,
                    dieslovaniId = result.Dieslovani?.ID
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _odstavkyService.DeleteOdstavkaAsync(id);
            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
        }
        [HttpPost]
        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            var (totalRecords, data) = await _odstavkyService.GetTableDataAsync(start, length);

            // Vracíme data ve formátu, který DataTables očekává
            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetTableDataOdDetail(int id)
        {
            var odstavkaList = await _odstavkyService.GetTableDataOdDetailAsync(id);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                data = odstavkaList
            });
        }
         [HttpGet]
        public async Task<IActionResult> DetailOdstavkyJson(int id)
        {
            var detailOdstavky = await _odstavkyService.DetailOdstavkyJsonAsync(id);
            if(detailOdstavky==null)
            {
                return Json(new{
                    error="null"
                });
                
            }
            return Json(
            new
            {
                data=detailOdstavky
            });
        }
        public async Task<IActionResult> DetailOdstavky(int id)
        {
            var detail = await _odstavkyService.DetailOdstavkyAsync(id);
            if (detail == null)
                return NotFound();
            return View(detail);
        }

    }
    
}