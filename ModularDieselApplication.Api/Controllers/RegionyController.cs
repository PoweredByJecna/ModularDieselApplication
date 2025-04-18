using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;

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
            var regionInfo = await _regionyService.GetRegionDataPrahaAsync();
            return Json(new
            {
                data = regionInfo
            });
        }

        // ----------------------------------------
        // Fetch region data for Severni Morava.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataSeverniMorava()
        {
            var regionInfo = await _regionyService.GetRegionDataSeverniMoravaAsync();
            return Json(new
            {
                data = regionInfo
            });
        }

        // ----------------------------------------
        // Fetch region data for Jizni Morava.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataJizniMorava()
        {
            var regionInfo = await _regionyService.GetRegionDataJizniMoravaAsync();
            return Json(new
            {
                data = regionInfo
            });
        }

        // ----------------------------------------
        // Fetch region data for Zapadni Cechy.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataZapadniCechy()
        {
            var regionInfo = await _regionyService.GetRegionDataZapadniCechyAsync();
            return Json(new
            {
                data = regionInfo
            });
        }

        // ----------------------------------------
        // Fetch region data for Severni Cechy.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataSeverniCechy()
        {
            var regionInfo = await _regionyService.GetRegionDataSeverniCechyAsync();
            return Json(new
            {
                data = regionInfo
            });
        }

        // ----------------------------------------
        // Fetch region data for Jizni Cechy.
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetRegionDataJizniCechy()
        {
            var regionInfo = await _regionyService.GetRegionDataJizniCechyAsync();
            return Json(new
            {
                data = regionInfo
            });
        }
    }
}