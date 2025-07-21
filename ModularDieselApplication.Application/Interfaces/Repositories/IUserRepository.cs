using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByIdAsync(string userId);
        Task<string> GetUserPrimaryRoleAsync(string userId);
        Task<Pohotovosti?> GetUserPohotovostAsync(string userId);
        Task<Technik> GetUserTechnikAsync(string userId);
        Task<Region> GetUserRegionForFirmaAsync(string firmaId);
        Task<SignInResult> LoginAsync(string username, string password, bool rememberMe);
        Task LogoutAsync();
        Task<List<object>> GetDieslovaniByUserId(string userId);
        Task<HandleResult> AddAsync(User user);
        Task<HandleResult> ChangePasswordAsync(string userId, string newPassword);
    }
}
