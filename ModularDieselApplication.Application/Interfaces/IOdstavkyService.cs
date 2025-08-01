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
        Task<HandleResult> CreateNewOdstavka(string lokalita, DateTime od, DateTime do_, string popis);
        Task<HandleResult> ActionMethods(ServiceFilterEnum serviceFilter, ActionFilter filter, string Id, DateTime time = default, User? currentUser = null);
        Task<List<Odstavka>> GetTableData();

    }
}