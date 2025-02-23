using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ILokalityService : ILokalityRepository
    {
        Task<List<object>> GetAllLokalityAsync();
    }
}