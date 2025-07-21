using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Enum;

namespace Diesel_modular_application.Controllers
{
    public class RegionyController : Controller
    {
        private readonly IRegionyService _regionyService;

        public RegionyController(IRegionyService regionyService)
        {
            _regionyService = regionyService;
        }
        // ----------------------------------------
        // Render the main view for Regiony.
        // ----------------------------------------
        [Authorize]
        public IActionResult Index()
        {
            return View();
        }
        // ----------------------------------------
        // Fetch region data for Praha.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataPraha()
        {
            return await RegionInfo(RegionyFilterEnum.Praha);
        }
        // ----------------------------------------
        // Fetch region data for Severni Morava.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataSeverniMorava()
        {
            return await RegionInfo(RegionyFilterEnum.SeverniMorava);
        }
        // ----------------------------------------
        // Fetch region data for Jizni Morava.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataJizniMorava()
        {
            return await RegionInfo(RegionyFilterEnum.JizniMorava);
        }
        // ----------------------------------------
        // Fetch region data for Zapadni Cechy.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataZapadniCechy()
        {
            return await RegionInfo(RegionyFilterEnum.ZapadniCechy);
        }
        // ----------------------------------------
        // Fetch region data for Severni Cechy.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataSeverniCechy()
        {
            return await RegionInfo(RegionyFilterEnum.SeverniCechy);
        }
        // ----------------------------------------
        // Fetch region data for Jizni Cechy.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataJizniCechy()
        {
            return await RegionInfo(RegionyFilterEnum.JizniCechy);
        }
        private async Task<IActionResult> RegionInfo(RegionyFilterEnum filter)
        {
            var regionInfo = await _regionyService.GetRegionData(filter);
            return Json(new { data = regionInfo });
        }
    }
}