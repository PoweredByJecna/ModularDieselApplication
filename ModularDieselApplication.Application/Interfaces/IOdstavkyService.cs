using Microsoft.AspNetCore.Authentication;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IOdstavkyService
    {
        Task<List<string>> SuggestLokalitaAsync(string query);
        Task<Odstavka> GetOdstavka(GetOdstavka filter, object value);
        Task DeleteOdstavkaAsync(string idodstavky);
        Task ChangeTimeOdstavkyAsync(string idodstavky, DateTime time, ActionFilter filter);
        Task<HandleResult> CreateNewOdstavka(string lokalita, DateTime od, DateTime do_, string popis);

    }
}