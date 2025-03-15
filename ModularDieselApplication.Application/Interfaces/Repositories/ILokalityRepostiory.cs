using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface ILokalityRepository
    {
        Task<List<object>> GetAllLokalityAsync();
        Task<Lokalita> GetLokalitaByName(string nazev);
        Task<Lokalita> DetailLokalityAsync(string nazev);
        Task<List<object>> GetDieslovaniNaLokaliteAsync(string nazev);
        Task<List<object>> GetOdstavkynaLokaliteAsync(string nazev);
    }
}