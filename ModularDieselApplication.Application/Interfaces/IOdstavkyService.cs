using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IOdstavkyService
    {
        Task<List<string>> SuggestLokalitaAsync(string query);
        Task<Odstavka> GetOdstavka(GetOdstavka filter, object value);
        Task DeleteOdstavkaAsync(string idodstavky);
        Task ChangeTimeOdstavkyAsync(string idodstavky, DateTime time, ActionFilter filter);
        Task<Odstavka> CreateNewOdstavka(Lokalita lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis);

    }
}