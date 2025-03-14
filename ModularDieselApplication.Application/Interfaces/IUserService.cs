using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IUserService
    {
        Task<object> DetailUserJsonAsync(string userId);
        Task<bool> IsUserInRoleAsync(string userId, string roleName);
        Task<List<object>> VazbyJsonAsync(string userId);
    }
}
