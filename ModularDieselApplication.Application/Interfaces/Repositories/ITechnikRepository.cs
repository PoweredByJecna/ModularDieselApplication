

using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Interfaces.Repositories
{
    public interface ITechniciRepository
    {
        Task<Technik?> GetByIdAsync(string idTechnika);
        Task<Technik?> GetByUserIdAsync(string idUser);
        Task<Technik> GetByFirmaIdAsync(int idFirmy);
        Task<bool> IsTechnikOnDutyAsync(string idTechnika);
        Task UpdateAsync(Technik technik);
    }
}