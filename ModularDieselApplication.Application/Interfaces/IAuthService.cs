using Microsoft.AspNetCore.Identity;
namespace ModularDieselApplication.Application.Interfaces

{
    public interface IAuthService
    {
        Task<SignInResult> LoginAsync(string username, string password, bool rememberMe);
        
    }
}