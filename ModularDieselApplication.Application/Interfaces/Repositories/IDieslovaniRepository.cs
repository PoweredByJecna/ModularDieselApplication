
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IDieslovaniRepository
    {
        Task<Dieslovani> GetDaAsync(GetDA filter, object value);
        Task<bool> AnotherDieselRequest(string idTechnika);
        IQueryable<Dieslovani> GetDieslovaniQuery(User? currentUser = null, bool isEngineer = false);
        Task<HandleResult> AddAsync(Dieslovani dieslovani);
        Task UpdateAsync(Dieslovani dieslovani);
        Task<bool> DeleteAsync(string id);
        Task<List<Dieslovani>> GetAnotherDA(Dieslovani dieslovani);
    }
}