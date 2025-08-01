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
        Task VstupAsync(string idDieslovani);
        Task OdchodAsync(string idDieslovani);
        Task<HandleResult> TakeAsync(string idDieslovani, User currentUser);
        Task DeleteDieslovaniAsync(string id);
        Task ChangeTimeAsync(string idDieslovani, DateTime time, ActionFilter type);
        Task<HandleResult<Dieslovani>> HandleOdstavkyDieslovani(Odstavka newOdstavka);
        Task<Technik> AssignTechnikAsync(Dieslovani dieslovani);
        Task<Dieslovani> CreateNewDieslovaniAsync(Odstavka newOdstavka, Technik technik);
        Task CallDieslovaniAsync(string idodstavky);
        Task<List<Dieslovani>> GetTableData(DieslovaniOdstavkaFilterEnum filter, User currentUser = null, bool isEngineer = default);

    }
    
}