
using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IDieslovaniRepository
    {
        Task<Dieslovani?> GetByIdAsync(string id);
        Task<bool> AnotherDieselRequest(string idTechnika);
        IQueryable<Dieslovani> GetDieslovaniQuery();       
        Task<List<object>> GetDieslovaniDataAsync(IQueryable<Dieslovani> query);
        Task AddAsync(Dieslovani dieslovani);
        Task UpdateAsync(Dieslovani dieslovani);
        Task<bool> DeleteAsync(string id);
        Task<Dieslovani?> GetDieslovaniWithTechnikAsync(string firmaId);
        Task<Dieslovani?> GetTechnikByIdAsync(string technikId);
        Task UpdateDieslovaniAsync(Dieslovani dieslovani);
        Task <Dieslovani> GetDAbyOdstavkaAsync(string idOdstavky);
        Task<string> GetIDbyDieselId(string idDieslovani);
        Task<Odstavka> GetByOdstavkaByIdAsync(string idodstavky);

    }
}