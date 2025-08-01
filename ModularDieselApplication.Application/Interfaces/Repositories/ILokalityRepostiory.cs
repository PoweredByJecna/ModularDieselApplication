using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface ILokalityRepository
    {
        Task<List<Lokalita>> GetAllLokalityAsync();
        Task<Lokalita> GetLokalitaByName(string nazev);
        Task<Lokalita> DetailLokalityAsync(string nazev);
        Task<List<Dieslovani>> GetDieslovaniNaLokaliteAsync(string nazev);
        Task<List<Odstavka>> GetOdstavkynaLokaliteAsync(string nazev);
    }
}