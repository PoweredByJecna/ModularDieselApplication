using System.Collections.Generic;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IRegionyService
    {
        Task<List<object>> GetRegionDataPrahaAsync();
        Task<List<object>> GetRegionDataSeverniMoravaAsync();
        Task<List<object>> GetRegionDataJizniMoravaAsync();
        Task<List<object>> GetRegionDataJizniCechyAsync();
        Task<List<object>> GetRegionDataSeverniCechyAsync();
        Task<List<object>> GetRegionDataZapadniCechyAsync();
        Task<List<object>> GetRegionByIdFirmy(string id);
        Task<Firma> GetFirmaVRegionuAsync(string idReg);
    }
}