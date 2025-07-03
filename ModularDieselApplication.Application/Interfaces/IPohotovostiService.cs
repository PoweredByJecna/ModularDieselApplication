using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IPohotovostiService
    {
        Task<List<Pohotovosti>> GetAllPohotovostiAsync();
        Task<bool> PohovostiVRegionuAsync(string firmy, DateTime OD, DateTime DO);
        Task<HandleResult> ZapisPohotovostAsync(DateTime zacatek, DateTime konec, User currentUser);
        Task<string> GetTechnikActivTechnikByIdFirmaAsync(string idFirmy, DateTime OD, DateTime DO);
        Task<(int totalRecords, List<object> data)> GetPohotovostTableDataAsync(int start, int length);
    }
}