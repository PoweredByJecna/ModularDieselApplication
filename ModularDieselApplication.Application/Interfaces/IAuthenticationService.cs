
namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
       Task<bool> ValidateUserAsync(string userName, string password);
       
    }
}