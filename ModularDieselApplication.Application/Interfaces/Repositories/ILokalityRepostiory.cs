using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface ILokalityRepository
    {
        Task<List<object>> GetAllLokalityAsync();
        Task<Lokalita> GeLokalitaByID(int id);
        Task<Lokalita> DetailLokalityAsync(int id);
    }
}