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
        Task<HandleOdstavkyDieslovaniResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option);
        Task<HandleOdstavkyDieslovaniResult> TestOdstavkaAsync();
        Task<Odstavka> DetailOdstavkyAsync(int id);
        Task<object> DetailOdstavkyJsonAsync(int id);
        Task<HandleOdstavkyDieslovaniResult> UpdateOdstavkaAsync(int idodstavky, string lokalita, DateTime od, DateTime @do, string popis);
        Task<HandleOdstavkyDieslovaniResult> DeleteOdstavkaAsync(int idodstavky);
        Task<(int totalRecords, List<object> data)> GetTableDataAsync(int start = 0, int length = 0);
        Task<List<object>> GetTableDataOdDetailAsync(int dieslovaniId);
    }
}