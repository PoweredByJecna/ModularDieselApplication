using System.Xml.XPath;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Application.Interfaces;

namespace ModularDieselApplication.Api.Controllers
{
        [Authorize]
        public class DieslovaniController(
            IDieslovaniService _dieslovaniService,
            UserManager<TableUser> _userManager,
            IMapper _mapper,
            IServiceBaseClass _serviceBaseClass
        ) : Controller
        {
        [HttpGet] public IActionResult Index() => View();

        [HttpPost]
        public async Task<IActionResult> Vstup(string IdDieslovani)=>
        JsonResult(await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Dieslovani, ActionFilter.Vstup,IdDieslovani));

        [HttpPost]
        public async Task<IActionResult> Take(string IdDieslovani)
        {
            var domainUser = _mapper.Map<User>(await _userManager.GetUserAsync(User));
            return JsonResult(await _dieslovaniService.TakeAsync(IdDieslovani, domainUser));
        }
        [HttpPost]
        public async Task<IActionResult> Odchod(string IdDieslovani)
        {
            var result = await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Dieslovani,ActionFilter.Odchod,IdDieslovani);
            return JsonResult(result);
        }
        [HttpGet] public async Task<IActionResult> GetTableDataRunningTable()=>await InfoDataTable(DieslovaniOdstavkaFilterEnum.RunningTable);
        [HttpGet] public async Task<IActionResult> GetTableDataAllTable() =>  await InfoDataTable(DieslovaniOdstavkaFilterEnum.AllTable);
        [HttpGet] public async Task<IActionResult> GetTableUpcomingTable() => await InfoDataTable(DieslovaniOdstavkaFilterEnum.UpcomingTable);    
        [HttpGet] public async Task<IActionResult> GetTableDataEndTable() => await InfoDataTable(DieslovaniOdstavkaFilterEnum.EndTable);
        [HttpGet] public async Task<IActionResult> GetTableDatathrashTable()=> await InfoDataTable(DieslovaniOdstavkaFilterEnum.TrashTable);
        [HttpPost] [Authorize(Roles = "Admin")] public async Task<IActionResult> Delete(string id) => JsonResult(await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Dieslovani, ActionFilter.Delete, id));
        [HttpPost] public async Task<IActionResult> ChangeTime(string dieslovaniId, DateTime time)=> JsonResult(await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Dieslovani,ActionFilter.ChangeTimeZactek,dieslovaniId, time));
        [HttpPost] public async Task<IActionResult> CallDieslovani(string odstavkaId)=> JsonResult( await _serviceBaseClass.ActionMethods(ServiceFilterEnum.Dieslovani, ActionFilter.CallDA, odstavkaId));
        
        private JsonResult JsonResult(HandleResult result) =>
        Json(new { success = result.Success, message = result.Message });

        private async Task<IActionResult> InfoDataTable(DieslovaniOdstavkaFilterEnum filter)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var isEngineer = currentUser != null && await _userManager.IsInRoleAsync(currentUser, "Engineer");
            var domainUser = _mapper.Map<User>(currentUser);
            var data = await _dieslovaniService.GetTableData(filter, domainUser, isEngineer);
            return Json(new { data });
        }

    }
}
