using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using static ModularDieselApplication.Application.Services.OdstavkyService;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IDieslovaniService
    {
        
        Task<List<object>> GetTableData(DieslovaniFilterEnum filter, User currentUser, bool isEngineer);
        Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result);
        Task<HandleResult> VstupAsync(string idDieslovani);
        Task<HandleResult> OdchodAsync(string idDieslovani);
        Task<HandleResult> TakeAsync(string idDieslovani, User user);
        Task<Dieslovani?> DetailDieslovaniAsync(string id);
        Task<object> DetailDieslovaniJsonAsync(string id);
        Task<Dieslovani>GetDieslovaniByOdstavkaId(string id);
        Task<HandleResult> DeleteDieslovaniAsync(string iDdieslovani);
        Task<string> GetOdstavkaIDbyDieselId(string idDieslovani);
        Task<List<object>> GetTableDataDetailJsonAsync(string id);
        Task<bool>AnotherDieselRequestAsync(string idTechnika);  
        Task<HandleResult> ChangeTimeAsync(string idDieslovani, DateTime time, string type);
        Task<HandleResult> CallDieslovaniAsync(string odstavky);
    }
}