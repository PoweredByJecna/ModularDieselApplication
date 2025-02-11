using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string userId);
        Task<string?> GetUserPrimaryRoleAsync(string userId);
        Task<Pohotovosti?> GetUserPohotovostAsync(string userId);
        Task<Technik?> GetUserTechnikAsync(string userId);
        Task<Region?> GetUserRegionForFirmaAsync(int firmaId);
    }
}
