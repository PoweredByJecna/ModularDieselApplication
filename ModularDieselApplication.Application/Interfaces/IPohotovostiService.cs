using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IPohotovostiService
    {
        Task<List<Pohotovosti>> GetAllPohotovostiAsync();
        Task<bool> PohovostiVRegionuAsync(int firmy);
        Task<(bool Success, string Message)> ZapisPohotovostAsync(Pohotovosti pohotovosti, User currentUser);
        Task<Technik> GetTechnikActivTechnikByIdFirmaAsync(int idFirmy);
        Task<(int totalRecords, List<object> data)> GetPohotovostTableDataAsync(int start, int length);
    }
}