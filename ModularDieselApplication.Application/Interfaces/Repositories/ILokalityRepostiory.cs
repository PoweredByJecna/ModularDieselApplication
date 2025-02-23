using ModularDieselApplication.Domain.Entities;
namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface ILokalityRepository
    {
        Task<List<object>> GetAllLokalityAsync();
    }
}