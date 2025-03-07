using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Repositories;
using Microsoft.AspNetCore.Identity;
namespace ModularDieselApplication.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        public AuthService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }
        public async Task<SignInResult> LoginAsync(string username, string password, bool rememberMe)
        {
            return await _userRepository.LoginAsync(username, password, rememberMe);
        }
        public async Task LogoutAsync()
        {
            await _userRepository.LogoutAsync();
        }
    }
}