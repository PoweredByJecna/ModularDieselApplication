using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IPohotovostiService
    {
        Task<List<Pohotovosti>> GetAllPohotovostiAsync();
        Task<bool> PohovostiVRegionuAsync(int firmy);
        Task<HandleResult> ZapisPohotovostAsync(Pohotovosti pohotovosti, User currentUser);
        Task<string> GetTechnikActivTechnikByIdFirmaAsync(int idFirmy);
        Task<(int totalRecords, List<object> data)> GetPohotovostTableDataAsync(int start, int length);
    }
}