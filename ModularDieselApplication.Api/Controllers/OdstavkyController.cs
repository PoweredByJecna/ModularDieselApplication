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
            return Json(lokalities);
        }

        // ----------------------------------------
        // Create a new Odstavka.
        // ----------------------------------------
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

        // ----------------------------------------
        // Test the Odstavka functionality.
        // ----------------------------------------
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

        // ----------------------------------------
        // Delete an Odstavka by ID.
        // ----------------------------------------
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

        // ----------------------------------------
        // Fetch Odstavky data for a table.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> GetTableData(int start = 0, int length = 0)
        {
            var (totalRecords, data) = await _odstavkyService.GetTableDataAsync(start, length);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }

        // ----------------------------------------
        // Fetch detailed Odstavky data for a table.
        // ----------------------------------------
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

        // ----------------------------------------
        // Fetch Odstavka details as JSON.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> DetailOdstavkyJson(int id)
        {
            var detailOdstavky = await _odstavkyService.DetailOdstavkyJsonAsync(id);
            if (detailOdstavky == null)
            {
                return Json(new { error = "null" });
            }
            return Json(new { data = detailOdstavky });
        }

        // ----------------------------------------
        // Render the detail view for an Odstavka.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> DetailOdstavky(int id)
        {
            var detail = await _odstavkyService.DetailOdstavkyAsync(id);
            if (detail == null)
                return NotFound();
            return View(detail);
        }

        // ----------------------------------------
        // Change the time of an Odstavka.
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> ChangeTimeOdstavky(int odstavkaId, DateTime time, string type)
        {
            var result = await _odstavkyService.ChangeTimeOdstavkyAsync(odstavkaId, time, type);
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