using ModularDieselApplication.Domain.Entities;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Interfaces
{
    public interface IOdstavkyRepository
    {
        Task<Odstavka?> GetByIdAsync(int id);
        Task<Lokalita?> GetByNameAsync(string name); 
        Task<List<Odstavka>> GetByLokalitaAsync(string name);
        Task<Lokalita> GetLokalityByIdAsync(int id);
        Task<List<Odstavka>> GetAllAsync();
        Task<int> GetOdstavkaCountAsync();
        Task AddAsync(Odstavka odstavka);
        Task UpdateAsync(Odstavka odstavka);
        Task DeleteAsync(int id);
        IQueryable<Odstavka> GetOdstavkaQuery();
        Task<Odstavka?> AnotherOdsatvkaAsync(int LokalitaId, DateTime od);

        Task<List<object>> GetOdstavkaDataAsync(IQueryable<Odstavka> query);


    }
}