using ModularDieselApplication.Domain.Entities;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ITechnikService
    {
        Task<Technik> GetTechnikByIdAsync(string technik);
        Task<Technik> GetTechnikByUserIdAsync(string idUser);
        Task<bool> IsTechnikOnDutyAsync(string idTechnika);
        Task UpdateTechnikAsync(Technik technik);
    }
}