using Microsoft.AspNetCore.Mvc;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using System.Threading.Tasks;



namespace ModularDieselApplication.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmailController : ControllerBase
    {
        private readonly IEmailService _emailService;

        public EmailController(IEmailService emailService)
        {
            _emailService = emailService;
        }

        [HttpPost("send")]
        public async Task<IActionResult> SendEmail([FromBody] Dieslovani dieslovani,string emailResult)
        {
            // Zavoláme metodu, která v EmailService sestaví 
            // text a předmět a e-mail odešle
            await _emailService.SendDieslovaniEmailAsync(dieslovani, emailResult);

            // Vrátíme 200 OK
            return Ok(new { message = "E‑mail byl odeslán." });
        }
    }
}
