
using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IDieslovaniRepository
    {
        Task<Dieslovani?> GetByIdAsync(int id);
        Task<bool> AnotherDieselRequest(string idTechnika);
        IQueryable<Dieslovani> GetDieslovaniQuery();       
        Task<List<object>> GetDieslovaniDataAsync(IQueryable<Dieslovani> query);
        Task AddAsync(Dieslovani dieslovani);
        Task UpdateAsync(Dieslovani dieslovani);
        Task<int> GetCountAsync();
        Task<bool> DeleteAsync(int id);
        Task<Dieslovani?> GetDieslovaniWithTechnikAsync(int firmaId);
        Task<Dieslovani?> GetTechnikByIdAsync(string technikId);
        Task UpdateDieslovaniAsync(Dieslovani dieslovani);
        Task <Dieslovani> GetDAbyOdstavkaAsync(int idOdstavky);
        Task<int> CountAsync(IQueryable<Dieslovani> query);
        Task<int> GetIDbyDieselId(int idDieslovani);

    }
}