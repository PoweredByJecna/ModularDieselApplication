using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IRegionyRepository
    {
        Task<List<object>> GetData(int regionId);
        Task<bool> TechnikHasPohotovost(string idTechnik);
        Task<List<object>> GetRegion(int firmaId);
        Task<Firma> GetFirmaAsync(int idReg);
        Task<int> GetLokalityCountAsync(int regionId);
        Task<int> GetOdstavkyCountAsync(int regionId);
        Task<List<Region>> GetRegionById (int idregion);
        Task<bool> GetValueIfTechnikHasPohotovostAsync(string idTechnik);
        Task<List<Technik>> GetTechnikListVRegionu (int IDfirmy);

   


    }
}