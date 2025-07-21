using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Enum;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IRegionyService
    {
        Task<List<object>> GetRegionData(RegionyFilterEnum regionFilter);
        Task<List<object>> GetRegionByIdFirmy(string id);
        Task<Firma> GetFirmaVRegionuAsync(string idReg);
    }
}