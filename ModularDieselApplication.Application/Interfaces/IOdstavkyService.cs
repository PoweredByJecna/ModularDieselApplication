using ModularDieselApplication.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Objects;
using static ModularDieselApplication.Application.Services.OdstavkyService;

namespace ModularDieselApplication.Application.Interfaces.Services
{
    public interface IOdstavkyService
    {
        Task<List<string>> SuggestLokalitaAsync(string query);
        Task<HandleResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option);
        Task<object> DetailOdstavkyAsync(string id);
        Task<object> DetailOdstavkyJsonAsync(string id);
        Task<HandleResult> DeleteOdstavkaAsync(string idodstavky);
        Task<List<object>> GetTableDataAsync();
        Task<List<object>> GetTableDataOdDetailAsync(string dieslovaniId);
        Task<HandleResult> ChangeTimeOdstavkyAsync(string idodstavky, DateTime time, string type);
    }
}