using ModularDieselApplication.Domain.Entities;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Enum;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ITechnikService
    {
        Task<Technik> GetTechnik(GetTechnikEnum getTecnikEnum, string? idTechnika = null, string? idUser = null, string? idFirmy = null);
        Task<bool> IsTechnikOnDutyAsync(string idTechnika);
        Task UpdateTechnikAsync(Technik technik);
    }
}