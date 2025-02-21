using ModularDieselApplication.Application.Interfaces.Repositories;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ILokalityService : ILokalityRepository
    {
        Task<List<Lokalita>> GetAllLokalityAsync();
    }
}