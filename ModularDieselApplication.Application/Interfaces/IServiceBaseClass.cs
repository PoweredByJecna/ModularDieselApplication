using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces
{
    public interface IServiceBaseClass 
    {
        Task<List<object>> GetTableData(ServiceFilterEnum serviceFilter, DieslovaniOdstavkaFilterEnum filter, User currentUser = null, bool isEngineer = default);
        Task<HandleResult> ActionMethods(ServiceFilterEnum serviceFilter, ActionFilter filter, string Id, DateTime time = default, User? currentUser = null);
    }
}