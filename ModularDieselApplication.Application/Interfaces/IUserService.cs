using System.Reflection.Metadata;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<object> DetailUserJsonAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
        Task<List<object>> VazbyJsonAsync(string userId);
        Task<HandleResult> AddUserAsync(string username, string password, string email, string role, string Jmeno, string Prijmeni);
        Task<HandleResult> ChangePasswordAsync(string userId, string newPassword);
    }
}
