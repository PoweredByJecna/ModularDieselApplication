
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
namespace ModularDieselApplication.Infrastructure.Identity
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly UserManager<TableUser> _userManager;

        public AuthenticationService(UserManager<TableUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<bool> ValidateUserAsync(string userName, string password)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null) return false;

            return await _userManager.CheckPasswordAsync(user, password);
        }
    }
}
