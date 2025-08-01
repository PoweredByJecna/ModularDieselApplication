using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ILokalityService
    {
        Task<List<Lokalita>> GetAllLokalityAsync();
        Task<Lokalita> DetailLokalityAsync(string nazev);
        Task<object> DetailLokalityJsonAsync(string nazev);
        Task<List<object>> GetDieslovaniNaLokaliteAsync(string nazev);
        Task<List<object>> GetOdstavkynaLokaliteAsync(string nazev);

    }
}