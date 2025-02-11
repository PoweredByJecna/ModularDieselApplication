using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IEmailService
    {
        Task SendDieslovaniEmailAsync(Dieslovani dieslovani, string emailResult);
        Task SendEmailAsync(string subject, string body);
    }
}