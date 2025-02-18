using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using static ModularDieselApplication.Application.Services.OdstavkyService;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IDieslovaniService
    {
        Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(User currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(User currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableDataUpcomingTableAsync(User currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(User currentUser, bool isEngineer);
        Task<(int totalRecords, List<object> data)> GetTableDatathrashTableAsync(User currentUser, bool isEngineer);
        Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky);
        Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result);
        Task<(bool Success, string Message)> VstupAsync(int idDieslovani);
        Task<(bool Success, string Message)> OdchodAsync(int idDieslovani);
        Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani);
        Task<(bool Success, string Message, string? TempMessage)> TakeAsync(int idDieslovani, User user);
        Task<Dieslovani?> DetailDieslovaniAsync(int id);
        Task<object> DetailDieslovaniJsonAsync(int id);
        Task<Dieslovani>GetDieslovaniByOdstavkaId(int id);
        Task<(bool Success, string Message)> DeleteDieslovaniAsync(int iDdieslovani);
    }
}