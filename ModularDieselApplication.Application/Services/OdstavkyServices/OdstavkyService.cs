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
        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
           return await _odstavkyQueryService.SuggestLokalitaAsync(query);
        }
        public async Task<HandleResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option)
        {
            return await _odstavkaAssignmentService.CreateOdstavkaAsync(lokalita, od, @do, popis, option);
        }

        public async Task<HandleResult> TestOdstavkaAsync()
        {
            var result = await _odstavkaAssignmentService.TestOdstavkaAsync();
            return result;
        }

        public async Task<Odstavka> DetailOdstavkyAsync(int id)
        {
            var odstavka = await _odstavkaRepository.GetByIdAsync(id);
            var query = _odstavkaRepository.GetOdstavkaQuery();
            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query.Where(o => o.ID == id));
            if (odstavka == null)
            {
                throw new InvalidOperationException($"Odstavka with id {id} not found.");
            }
            return odstavka;
        }

        public async Task<object> DetailOdstavkyJsonAsync(int id)
        {
            return await _odstavkyQueryService.DetailOdstavkyJsonAsync(id);
        }

        public async Task<HandleResult> DeleteOdstavkaAsync(int idodstavky)
        {
            return await _odstavkyActionService.DeleteOdstavkaAsync(idodstavky);
        }
        public async Task<(int totalRecords, List<object> data)> GetTableDataAsync(int start = 0, int length = 0)
        {
           return await _odstavkyQueryService.GetTableDataAsync(start, length);
        }

        public async Task<List<object>> GetTableDataOdDetailAsync(int idDieslovani)
        {
            return await _odstavkyQueryService.GetTableDataOdDetailAsync(idDieslovani);
        }

        public Task<HandleResult> UpdateOdstavkaAsync(int idodstavky, string lokalita, DateTime od, DateTime @do, string popis)
        {
            throw new NotImplementedException();
        }
        public async Task<HandleResult> ChangeTimeOdstavkyAsync(int idodstavky, DateTime time, string type)
        {
           return await  _odstavkyActionService.ChangeTimeOdstavkyAsync(idodstavky, time, type);
        }   
       
    }
}