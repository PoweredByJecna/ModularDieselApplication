using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Interfaces
{
    public interface IOdstavkyRepository
    {
        Task<Odstavka> GetOdstavkaAsync(GetOdstavka filter, object value);
        Task AddAsync(Odstavka odstavka);
        Task UpdateAsync(Odstavka odstavka);
        Task DeleteAsync(string id);
        IQueryable<Odstavka> GetOdstavkaQuery();
        Task<Odstavka> AnotherOdsatvkaAsync(string LokalitaId, DateTime od);


    }
}