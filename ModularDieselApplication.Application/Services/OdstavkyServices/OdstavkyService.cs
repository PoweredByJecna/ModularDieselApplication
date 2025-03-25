using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Services
{
    public class OdstavkyService : IOdstavkyService
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly OdstavkyActionService _odstavkyActionService;
        private readonly OdstavkyQueryService _odstavkyQueryService;
        private readonly OdstavkaAssignmentService _odstavkaAssignmentService;

        public OdstavkyService(IOdstavkyRepository odstavkaRepository, OdstavkyActionService odstavkyActionService, OdstavkyQueryService odstavkyQueryService, OdstavkaAssignmentService odstavkaAssignmentService)
        {
            _odstavkaRepository = odstavkaRepository;
            _odstavkyActionService = odstavkyActionService;
            _odstavkyQueryService = odstavkyQueryService;
            _odstavkaAssignmentService = odstavkaAssignmentService;
        }

        // ----------------------------------------
        // Suggest lokalita names based on a query.
        // ----------------------------------------
        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
            return await _odstavkyQueryService.SuggestLokalitaAsync(query);
        }

        // ----------------------------------------
        // Create a new odstávka.
        // ----------------------------------------
        public async Task<HandleResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option)
        {
            return await _odstavkaAssignmentService.CreateOdstavkaAsync(lokalita, od, @do, popis, option);
        }

        // ----------------------------------------
        // Test the creation of an odstávka.
        // ----------------------------------------
        public async Task<HandleResult> TestOdstavkaAsync()
        {
            return await _odstavkaAssignmentService.TestOdstavkaAsync();
        }

        // ----------------------------------------
        // Get detailed odstávka data.
        // ----------------------------------------
        public async Task<object> DetailOdstavkyAsync(int id)
        {
            var query = _odstavkaRepository.GetOdstavkaQuery();
            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query.Where(o => o.ID == id));
            if (data == null)
            {
                throw new InvalidOperationException($"Odstavka with id {id} not found.");
            }
            return data;
        }

        // ----------------------------------------
        // Get odstávka details as JSON.
        // ----------------------------------------
        public async Task<object> DetailOdstavkyJsonAsync(int id)
        {
            return await _odstavkyQueryService.DetailOdstavkyJsonAsync(id);
        }

        // ----------------------------------------
        // Delete an odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> DeleteOdstavkaAsync(int idodstavky)
        {
            return await _odstavkyActionService.DeleteOdstavkaAsync(idodstavky);
        }

        // ----------------------------------------
        // Get table data for odstávky.
        // ----------------------------------------
        public async Task<(int totalRecords, List<object> data)> GetTableDataAsync(int start = 0, int length = 0)
        {
            return await _odstavkyQueryService.GetTableDataAsync(start, length);
        }

        // ----------------------------------------
        // Get detailed table data for a specific dieslovani.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataOdDetailAsync(int idDieslovani)
        {
            return await _odstavkyQueryService.GetTableDataOdDetailAsync(idDieslovani);
        }

        // ----------------------------------------
        // Update an odstávka record.
        // ----------------------------------------
        public Task<HandleResult> UpdateOdstavkaAsync(int idodstavky, string lokalita, DateTime od, DateTime @do, string popis)
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------
        // Change the time for an odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> ChangeTimeOdstavkyAsync(int idodstavky, DateTime time, string type)
        {
            return await _odstavkyActionService.ChangeTimeOdstavkyAsync(idodstavky, time, type);
        }

        // ----------------------------------------
        // Get an odstávka record by ID.
        // ----------------------------------------
        public async Task<Odstavka> GetOdstavkaByIdAsync(int id)
        {
            return await _odstavkaRepository.GetByIdAsync(id);
        }
    }
}