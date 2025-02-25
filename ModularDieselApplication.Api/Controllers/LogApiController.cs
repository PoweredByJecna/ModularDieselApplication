using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Services;
using ModularDieselApplication.Interfaces;
public class LogApiController(ILogService logService) : ControllerBase
    {
        private readonly ILogService _logService = logService;

        // ----------------------------------------
        // Poslani logu pro dieslování do ajax
        // ----------------------------------------
        [HttpGet]
        public async Task<IActionResult> GetLogByEntity(int entityId)
        {   
            var logDieslovani = await _logService.GetLogByEntityAsync(entityId);
            return new JsonResult(new
            {
                data=logDieslovani
            });

        }
    }