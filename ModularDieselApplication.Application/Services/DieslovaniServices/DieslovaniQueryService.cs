
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
        public async Task<List<object>>GetDieslovaniByUserId (string UserID)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Technik.User.Id == UserID);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
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
                .Where(o => o.Odchod != DateTime.MinValue.Date && o.Odstavka.Do.Date <= DateTime.Today);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = await _dieslovaniRepository.CountAsync(query);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }
        /* ----------------------------------------
           GetTableDatathrashTableAsync
        ---------------------------------------- */
        public async Task<List<object>> GetTableDatathrashTableAsync(User? currentUser, bool isEngineer)
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
            }
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
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
        public async Task<List<object>> GetTableDataDetailJsonAsync(int id)
        {
            var detail = await _dieslovaniRepository.GetDAbyOdstavkaAsync(id);
            if (detail == null || detail.Odstavka == null || detail.Odstavka.Lokality == null || detail.Odstavka.Lokality.Region == null)
            {
                return new List<object>();
            }
            return new List<object>
            {
                new
                {
                    IDdieslovani = detail.ID,
                    lokalitaNazev = detail.Odstavka.Lokality.Nazev,
                    adresaLokality = detail.Odstavka.Lokality.Adresa,
                    distrib = detail.Odstavka.Distributor,
                    klasifikaceLokality = detail.Odstavka.Lokality.Klasifikace,
                    vstupnaLokalitu = detail.Vstup,
                    odchodzLokality = detail.Odchod,
                    userId = detail.Technik?.User?.Id ?? "Unknown",
                    jmenoTechnikaDA = detail.Technik?.User?.Jmeno ?? "Unknown",
                    prijmeniTechnikaDA = detail.Technik?.User?.Prijmeni ?? "Unknown",
                }
            };
        }
        public async Task<object> DetailDieslovaniJsonAsync(int id)
        {
            var detail = await _dieslovaniRepository.GetByIdAsync(id);
            if (detail == null)
            {
                return new
                {
                    error = "Dieslovani nenalezeno"
                };
            }
            else if (detail.Odstavka == null)
            {
                return new
                {
                    error = "Odstavka přiřazena k dieslovani nenalezena"
                };
            }
            else if (detail.Odstavka.Lokality == null)
            {
                return new
                {
                    error = "Lokalita přiřazena k dieslovani nenalezeno"
                };
            }
            else if (detail.Odstavka.Lokality.Region == null)
            {
                return new
                {
                    error = "Region přiřazen k dieslovani nenalezeno"
                };
            }
            return new 
            {
                dieslovaniId = detail.ID,
                odstavkaId = detail.Odstavka.ID,
                lokalita = detail.Odstavka.Lokality.Nazev,
                adresa = detail.Odstavka.Lokality.Adresa,
                klasifikace = detail.Odstavka.Lokality.Klasifikace,
                baterie = detail.Odstavka.Lokality.Baterie,
                region = detail.Odstavka.Lokality.Region.Nazev,
                popis = detail.Odstavka.Popis,
                Technik = detail.Technik?.User?.Id ?? "Unknown",
                jmenoTechnika = detail.Technik?.User?.Jmeno ?? "Unknown",
                prijmeniTechnika = detail.Technik?.User?.Prijmeni ?? "Unknown",
            };
        }
        
    }
}







