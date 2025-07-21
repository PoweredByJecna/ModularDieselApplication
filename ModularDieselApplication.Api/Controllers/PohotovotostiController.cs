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
   
    public class PohotovostiController : Controller
    {
        private readonly IPohotovostiService _pohotovostiService;
        private readonly UserManager<TableUser> _userManager;
        private readonly IMapper _mapper;

        public PohotovostiController(
            IPohotovostiService pohotovostiService,
            UserManager<TableUser> userManager,
            IMapper mapper)
        {
            _pohotovostiService = pohotovostiService;
            _userManager = userManager;
            _mapper = mapper;
        }
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        // ------------------------
        // Zapis
        // ------------------------
        [HttpPost]
        public async Task<IActionResult> Zapis(DateTime zacatek, DateTime Konec)
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var domainUser = _mapper.Map<User>(currentUser);

            var result  = await _pohotovostiService.ZapisPohotovostAsync(zacatek, Konec, domainUser);

            if (!result.Success)
            {
                return Json(new { success = false, message = result.Message });
            }
            else
            {
                return Json(new { success = true, message = result.Message });
            }
            
        }

        // ------------------------
        // GetTableDatapohotovostiTable
        // ------------------------¨
        [HttpGet]
        public async Task<IActionResult> GetTableDatapohotovostiTable(int start = 0, int length = 0)
        {
            var data = await _pohotovostiService.GetPohotovostTableDataAsync(start, length);
            return Json(new { data });
        }
    }
}
