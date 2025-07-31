using System.Xml.XPath;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;

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
        public IActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public async Task<IActionResult> GetTableDataDetailDieslovani(string id)
        {
            var detailDieslovani = await _dieslovaniService.GetTableDataDetailJsonAsync(id);
            if (detailDieslovani == null)
            {
                return Json(new
                {
                    error = "nenalezeno žádné dieslovaní"
                });
            }
            return Json(
            new
            {
                data = detailDieslovani
            });

        }
        // ----------------------------------------
        // Vstup - volá metodu ze servisu
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Vstup(string IdDieslovani)
        {
            var result = await _dieslovaniService.VstupAsync(IdDieslovani);
            return JsonResult(result);
        }
        [HttpPost]
        public async Task<IActionResult> Take(string IdDieslovani)
        {
            var domainUser = _mapper.Map<User>(await _userManager.GetUserAsync(User));
            var result = await _dieslovaniService.TakeAsync(IdDieslovani, domainUser);
            return JsonResult(result);
        }
        // ----------------------------------------
        // Odchod - volá metodu ze servisu
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> Odchod(string IdDieslovani)
        {
            var result = await _dieslovaniService.OdchodAsync(IdDieslovani);
            return JsonResult(result);
        }
        // ----------------------------------------
        // Načtení dat GetTableDataRunningTable
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableDataRunningTable()
        {
            return await InfoDataTable(DieslovaniFilterEnum.RunningTable);
        }
        // ----------------------------------------
        // Načtení dat GetTableDataAllTable
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableDataAllTable()
        {
            return await InfoDataTable(DieslovaniFilterEnum.AllTable);
        }

        // ----------------------------------------
        // Načtení dat GetTableDataUpcomingTable 
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetTableUpcomingTable()
        {
            return await InfoDataTable(DieslovaniFilterEnum.UpcomingTable);
        }
        // ----------------------------------------
        // Načtení dat GetTableDataEndTable 
        // ----------------------------------------        
        [HttpGet]
        public async Task<IActionResult> GetTableDataEndTable()
        {
            return await InfoDataTable(DieslovaniFilterEnum.EndTable);
        }
        // ----------------------------------------
        // Načtení dat GetTableThrashEndTable 
        // ----------------------------------------   
        [HttpGet]
        public async Task<IActionResult> GetTableDatathrashTable()
        {
            return await InfoDataTable(DieslovaniFilterEnum.TrashTable);
        }
        // ----------------------------------------
        // Smazání dieslování
        // ----------------------------------------
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _dieslovaniService.DeleteDieslovaniAsync(id);
            return JsonResult(result);
        }
        // ---------------------------------------- 
        // Změna času dieslovaní
        // ----------------------------------------
        [HttpPost]
        public async Task<IActionResult> ChangeTime(string dieslovaniId, DateTime time, string type)
        {
            var result = await _dieslovaniService.ChangeTimeAsync(dieslovaniId, time, type);
            return JsonResult(result);
        }
        // ---------------------------------------- 
        // Přiovolání dieslování
        // ----------------------------------------
        public async Task<IActionResult> CallDieslovani(string odstavkaId)
        {
            var result = await _dieslovaniService.CallDieslovaniAsync(odstavkaId);
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
        private async Task<IActionResult> InfoDataTable(DieslovaniFilterEnum filter)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);
            var data = await _dieslovaniService.GetTableData(filter, domainUser, isEngineer);
            return Json(new { data });
        }

    }
}
