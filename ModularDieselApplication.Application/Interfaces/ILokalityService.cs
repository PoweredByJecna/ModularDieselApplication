using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface ILokalityService
    {
        Task<List<Lokalita>> GetAllLokalityAsync();
        Task<List<Dieslovani>> GetDieslovaniNaLokaliteAsync(string nazev);
        Task<List<Odstavka>> GetOdstavkynaLokaliteAsync(string nazev);
        Task<Lokalita> GetLokalita(GetLokalita filter, object value);

    }
}