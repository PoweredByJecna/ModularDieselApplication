
using System.Collections.Generic;
using System.Reflection.Metadata;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IDieslovaniRepository
    {
        Task<Dieslovani> GetByIdAsync(string id);
        Task<bool> AnotherDieselRequest(string idTechnika);
        IQueryable<Dieslovani> GetDieslovaniQuery(User? currentUser = null, bool isEngineer = false);
        Task<HandleResult> AddAsync(Dieslovani dieslovani);
        Task UpdateAsync(Dieslovani dieslovani);
        Task<bool> DeleteAsync(string id);
        Task<Dieslovani?> GetDieslovaniWithTechnikAsync(string firmaId);
        Task UpdateDieslovaniAsync(Dieslovani dieslovani);
        Task <Dieslovani> GetDAbyOdstavkaAsync(string idOdstavky);
        Task<string> GetIDbyDieselId(string idDieslovani);
        Task<Odstavka> GetByOdstavkaByIdAsync(string idodstavky);

    }
}