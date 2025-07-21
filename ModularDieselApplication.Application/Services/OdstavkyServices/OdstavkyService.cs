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
        // Get detailed odstávka data.
        // ----------------------------------------
        public async Task<object> DetailOdstavkyAsync(string id)
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
        public async Task<object> DetailOdstavkyJsonAsync(string id)
        {
            return await _odstavkyQueryService.DetailOdstavkyJsonAsync(id);
        }

        // ----------------------------------------
        // Delete an odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> DeleteOdstavkaAsync(string idodstavky)
        {
            return await _odstavkyActionService.DeleteOdstavkaAsync(idodstavky);
        }

        // ----------------------------------------
        // Get table data for odstávky.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataAsync()
        {
            return await _odstavkyQueryService.GetTableDataAsync();
        }

        // ----------------------------------------
        // Get detailed table data for a specific dieslovani.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataOdDetailAsync(string idDieslovani)
        {
            return await _odstavkyQueryService.GetTableDataOdDetailAsync(idDieslovani);
        }

        // ----------------------------------------
        // Change the time for an odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> ChangeTimeOdstavkyAsync(string idodstavky, DateTime time, string type)
        {
            return await _odstavkyActionService.ChangeTimeOdstavkyAsync(idodstavky, time, type);
        }

        // ----------------------------------------
        // Get an odstávka record by ID.
        // ----------------------------------------
        public async Task<Odstavka> GetOdstavkaByIdAsync(string id)
        {
            return await _odstavkaRepository.GetByIdAsync(id);
        }
    }
}