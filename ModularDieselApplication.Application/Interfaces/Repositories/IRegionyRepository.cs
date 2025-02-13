using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IRegionyRepository
    {
        Task<List<object>> GetData(int regionId);
        Task<bool> TechnikHasPohotovost(string idTechnik);
        Task<List<object>> GetRegion(int firmaId);
        Task<Firma> GetFirmaAsync(int idReg);
   


    }
}