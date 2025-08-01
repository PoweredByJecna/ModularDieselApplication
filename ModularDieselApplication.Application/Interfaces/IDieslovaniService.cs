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
        Task<bool> AnotherDieselRequestAsync(string idTechnika);
        Task<HandleResult<Dieslovani>> HandleOdstavkyDieslovani(Odstavka newOdstavka);
        Task<Technik> AssignTechnikAsync(Dieslovani dieslovani);
        Task<Dieslovani> CreateNewDieslovaniAsync(Odstavka newOdstavka, Technik technik);
        Task<List<Dieslovani>> GetTableData(DieslovaniOdstavkaFilterEnum filter, User currentUser = null, bool isEngineer = default);
        Task<HandleResult> ActionMethods(ActionFilter filter, string Id, DateTime time = default, User? currentUser = null);
    }
    
}