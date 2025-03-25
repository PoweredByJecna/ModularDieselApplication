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

        // ----------------------------------------
        // Log in a user with the provided credentials.
        // ----------------------------------------
        public async Task<SignInResult> LoginAsync(string username, string password, bool rememberMe)
        {
            return await _userRepository.LoginAsync(username, password, rememberMe);
        }

        // ----------------------------------------
        // Log out the currently authenticated user.
        // ----------------------------------------
        public async Task LogoutAsync()
        {
            await _userRepository.LogoutAsync();
        }
    }
}