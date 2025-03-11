using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ILokalityService
    {
        Task<List<object>> GetAllLokalityAsync();
        Task<Lokalita> DetailLokalityAsync(int id);
        Task<object> DetailLokalityJsonAsync(int id);
    }
}