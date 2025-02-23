
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;


namespace ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniQueryService
{
    public class DieslovaniQueryService
    {

        private readonly IDieslovaniRepository _dieslovaniRepository;  
        private readonly IRegionyService _regionyService;
        private readonly ITechnikService _technikService;

        /* ----------------------------------------
           Kontrostruktor
        ---------------------------------------- */
        public DieslovaniQueryService(IDieslovaniRepository dieslovaniRepository, IRegionyService regionyService, ITechnikService technikService)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _regionyService = regionyService;
            _technikService = technikService;
        }
        /* ----------------------------------------
           GetTableDataRunningTableAsync
        ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery();
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = await _dieslovaniRepository.CountAsync(query);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        /* ----------------------------------------
           GetTableDataRunningTableAsync
        ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup != DateTime.MinValue && i.Odchod == DateTime.MinValue);
            int totalRecords = await _dieslovaniRepository.CountAsync(query);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        /* ----------------------------------------
           GetTableDataUpcomingTableAsync
        ---------------------------------------- */     
        public async Task<(int totalRecords, List<object> data)> GetTableDataUpcomingTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup ==DateTime.MinValue.Date && i.Odstavka.Od.Date==DateTime.Today && i.Technik.ID != "606794494");
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = await _dieslovaniRepository.CountAsync(query);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        /* ----------------------------------------
           GetTableDataEndTableAsync
        ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.Odchod != DateTime.MinValue.Date);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = await _dieslovaniRepository.CountAsync(query);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        /* ----------------------------------------
           GetTableDatathrashTableAsync
        ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDatathrashTableAsync(User? currentUser, bool isEngineer)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser), "Current user cannot be null.");
            }
            var technik = await _technikService.GetTechnikByUserIdAsync(currentUser.Id);

            if (technik == null || technik.Firma == null)
            {
                throw new InvalidOperationException("Technik or Firma is null.");
            }
            var firmaId = technik.Firma.ID;

            var validRegions = await _regionyService.GetRegionByIdFirmy(firmaId);

            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o=>o.Technik.ID=="606794494");

            if (isEngineer && validRegions.Any())
            {
                query = query.Where(d => validRegions.Contains(d.Odstavka.Lokality.Region.ID));
            }

            int totalRecords = await _dieslovaniRepository.CountAsync(query);

            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        /* ----------------------------------------
           GetTableDataOdDetailOdstavkyAsync
        ---------------------------------------- */
        public async Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.ID == idodstavky);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }
        /* ----------------------------------------
           Filtrace dat
        ---------------------------------------- */
        private static IQueryable<Dieslovani> FilteredData(IQueryable<Dieslovani> query, User? currentUser, bool isEngineer)
        {
            if (currentUser == null)
                return query;

            if (isEngineer)
            {
                var userId = currentUser.Id;
                query = query.Where(d => d.Technik.User.Id == userId);
            }
            return query;
        }
        
    }
}







