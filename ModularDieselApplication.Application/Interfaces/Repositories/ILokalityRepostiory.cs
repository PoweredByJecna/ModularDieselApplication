using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;
namespace ModularDieselApplication.Application.Interfaces.Repositories

{
    public interface ILokalityRepository
    {
        Task<List<Lokalita>> GetAllLokalityAsync();
        Task<Lokalita> GetLokalitaByName(string nazev);
        Task<Lokalita> DetailLokalityAsync(string nazev);
        Task<List<Dieslovani>> GetDieslovaniNaLokaliteAsync(string nazev);
        Task<List<Odstavka>> GetOdstavkynaLokaliteAsync(string nazev);
        Task<Lokalita> GetLokalitaAsync(GetLokalita filter, object value);
    }
}