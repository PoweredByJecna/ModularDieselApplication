using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IPohotovostiService
    {
        Task<bool> PohovostiVRegionuAsync(string firmy, DateTime OD, DateTime DO);
        Task<HandleResult> ZapisPohotovostAsync(DateTime zacatek, DateTime konec, User currentUser);
        Task<string> GetTechnikActivTechnikByIdFirmaAsync(string idFirmy, DateTime OD, DateTime DO);
        Task<List<object>> GetPohotovostTableDataAsync(int start, int length);
    }
}