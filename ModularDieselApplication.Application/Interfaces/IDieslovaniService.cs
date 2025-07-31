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
        Task<HandleResult<Dieslovani>> HandleOdstavkyDieslovani(Odstavka? newOdstavka);
        Task<List<Dieslovani>> GetTableData(DieslovaniFilterEnum filter, User currentUser, bool isEngineer);
        Task<bool> AnotherDieselRequestAsync(string idTechnika);  
        Task<HandleResult> ActioOnDieslovani(ActionFilter filter, string Id, DateTime time  = default);
    }
}