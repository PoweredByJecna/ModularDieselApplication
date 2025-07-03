using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IRegionyRepository
    {
        Task<List<object>> GetData(string regionId);
        Task<bool> TechnikHasPohotovost(string idTechnik);
        Task<List<object>> GetRegion(string firmaId);
        Task<Firma> GetFirmaAsync(string idReg);
        Task<int> GetLokalityCountAsync(string regionId);
        Task<int> GetOdstavkyCountAsync(string regionId);
        Task<List<Region>> GetRegionById (string idregion);
        bool GetValueIfTechnikHasPohotovost(string idTechnik);
        Task<List<Technik>> GetTechnikListVRegionu (string IDfirmy);

   


    }
}