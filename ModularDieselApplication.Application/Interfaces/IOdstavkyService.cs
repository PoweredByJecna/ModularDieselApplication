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
        Task<Odstavka> GetOdstavkaByIdAsync(int id);
        Task<HandleResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option);
        Task<HandleResult>TestOdstavkaAsync();
        Task<object> DetailOdstavkyAsync(int id);
        Task<object> DetailOdstavkyJsonAsync(int id);
        Task<HandleResult> UpdateOdstavkaAsync(int idodstavky, string lokalita, DateTime od, DateTime @do, string popis);
        Task<HandleResult> DeleteOdstavkaAsync(int idodstavky);
        Task<(int totalRecords, List<object> data)> GetTableDataAsync(int start = 0, int length = 0);
        Task<List<object>> GetTableDataOdDetailAsync(int dieslovaniId);
        Task<HandleResult> ChangeTimeOdstavkyAsync(int idodstavky, DateTime time, string type);
    }
}