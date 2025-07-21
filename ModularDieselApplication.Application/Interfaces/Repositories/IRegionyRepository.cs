using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IRegionyRepository
    {
        Task<Firma> GetFirmaAsync(string idReg);
        Task<(int pocetLokalit, int pocetOdstavek)> GetCountAsync(string name);
        Task<List<Region>> GetRegionByName(string name);
        bool GetValueIfTechnikHasPohotovost(string idTechnik);
        Task<List<Technik>> GetTechnikListVRegionu (string IDfirmy);

    }
}