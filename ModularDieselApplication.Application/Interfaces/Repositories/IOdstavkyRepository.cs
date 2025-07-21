using ModularDieselApplication.Domain.Entities;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Interfaces
{
    public interface IOdstavkyRepository
    {
        Task<Odstavka> GetByIdAsync(string id);
        Task<Lokalita?> GetByNameAsync(string name); 
        Task<List<Lokalita>> GetAllAsync();
        Task AddAsync(Odstavka odstavka);
        Task UpdateAsync(Odstavka odstavka);
        Task <bool>DeleteAsync(string id);
        IQueryable<Odstavka> GetOdstavkaQuery();
        Task<Odstavka?> AnotherOdsatvkaAsync(string LokalitaId, DateTime od);
        Task<List<object>> GetOdstavkaDataAsync(IQueryable<Odstavka> query);


    }
}