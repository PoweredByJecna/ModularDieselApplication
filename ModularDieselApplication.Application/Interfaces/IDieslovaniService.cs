using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using static ModularDieselApplication.Application.Services.OdstavkyService;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IDieslovaniService
    {
        Task<List<object>> GetTableDataAllTableAsync(User currentUser, bool isEngineer);
        Task<List<object>> GetTableDataRunningTableAsync(User currentUser, bool isEngineer);
        Task<List<object>> GetTableDataUpcomingTableAsync(User currentUser, bool isEngineer);
        Task<List<object>> GetTableDataEndTableAsync(User currentUser, bool isEngineer);
        Task<List<object>> GetTableDatathrashTableAsync(User currentUser, bool isEngineer);
        Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky);
        Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result);
        Task<HandleResult> VstupAsync(int idDieslovani);
        Task<HandleResult> OdchodAsync(int idDieslovani);
        Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani);
        Task<HandleResult> TakeAsync(int idDieslovani, User user);
        Task<Dieslovani?> DetailDieslovaniAsync(int id);
        Task<object> DetailDieslovaniJsonAsync(int id);
        Task<Dieslovani>GetDieslovaniByOdstavkaId(int id);
        Task<HandleResult> DeleteDieslovaniAsync(int iDdieslovani);
        Task<int> GetOdstavkaIDbyDieselId(int idDieslovani);
        Task<List<object>> GetTableDataDetailJsonAsync(int id);
        Task<bool>AnotherDieselRequestAsync(string idTechnika);  
        Task<HandleResult> ChangeTimeAsync(int idDieslovani, DateTime time, string type);
        Task<HandleResult> CallDieslovaniAsync(int odstavky);
    }
}