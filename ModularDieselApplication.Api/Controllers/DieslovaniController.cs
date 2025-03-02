using System.Xml.XPath;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Api.Controllers
{
    [Authorize]
    public class DieslovaniController : Controller
    {
        private readonly IDieslovaniService _dieslovaniService;
        private readonly IMapper _mapper;
        private readonly UserManager<TableUser> _userManager;

        
        public DieslovaniController(
            IDieslovaniService dieslovaniService,
            UserManager<TableUser> userManager,
            IMapper mapper)
        {
            _dieslovaniService = dieslovaniService;
            _userManager = userManager;
            _mapper = mapper;
        }
        // -------------------------------
        // Vracení pohledu
        // -------------------------------
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer"))
            {
                TempData["TableName"] = "Moje";
            }
            else
            {
                TempData["TableName"] = "";
            }
            return View();
        }
        // -------------------------------
        // Zobrazení detailu Dieslovani
        // -------------------------------
        [HttpGet]
        public async Task<IActionResult> DetailDieslovani(int id)
        {
            var detail = await _dieslovaniService.DetailDieslovaniAsync(id);
            if (detail == null)
                return NotFound();
            return View(detail);
        }
        // ----------------------------------------
        // Detail - načítá data pro dané Dieslovani
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> DetailDieslovaniJson(int id)
        {
            var detailDieslovani = await _dieslovaniService.DetailDieslovaniJsonAsync(id);
            if (detailDieslovani == null)
            {
                return Json(new{
                    error="null"
                });
            }
            return Json(
            new
            {
                data=detailDieslovani 
            });
        }
        // ----------------------------------------
        // Vstup - volá metodu ze servisu
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Vstup(int IdDieslovani)
        {
            var result = await _dieslovaniService.VstupAsync(IdDieslovani);

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
                });
            }
        }
        // ----------------------------------------
        // Odchod - volá metodu ze servisu
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Odchod(int IdDieslovani)
        {
            var result = await _dieslovaniService.OdchodAsync(IdDieslovani);

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
        // Načtení dat GetTableDataRunningTable
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableDataRunningTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);

            var (totalRecords, data) = await _dieslovaniService.GetTableDataRunningTableAsync(domainUser, isEngineer);


            // Vrátíme data ve formátu DataTables
            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        // ----------------------------------------
        // Načtení dat GetTableDataAllTable
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableDataAllTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);

            var (totalRecords, data) = await _dieslovaniService.GetTableDataAllTableAsync(domainUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
       
        // ----------------------------------------
        // Načtení dat GetTableDataUpcomingTable 
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableUpcomingTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);

            var (totalRecords, data) = await _dieslovaniService.GetTableDataUpcomingTableAsync(domainUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        // ----------------------------------------
        // Načtení dat GetTableDataEndTable 
        // ----------------------------------------        
        [HttpGet]
        public async Task<IActionResult> GetTableDataEndTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);


            var (totalRecords, data) = await _dieslovaniService.GetTableDataEndTableAsync(domainUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        // ----------------------------------------
        // Načtení dat GetTableThrashEndTable 
        // ----------------------------------------   
        [HttpGet]
        public async Task<IActionResult> GetTableDatathrashTable(int start = 0, int length = 0)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            bool isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);
            var (totalRecords, data) = await _dieslovaniService.GetTableDatathrashTableAsync(domainUser, isEngineer);

            return Json(new
            {
                draw = HttpContext.Request.Query["draw"].FirstOrDefault(),
                recordsTotal = totalRecords,
                recordsFiltered = totalRecords,
                data = data
            });
        }
        // ----------------------------------------
        // Smazání dieslování
        // ----------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            var (Success, Message) = await _dieslovaniService.DeleteDieslovaniAsync(id);
            if (!Success)
            {
                return Json(new { success = false, message = Message });
            }
            else
            {
                return Json(new { success = true, message = Message });
            }
        }
    }
}

