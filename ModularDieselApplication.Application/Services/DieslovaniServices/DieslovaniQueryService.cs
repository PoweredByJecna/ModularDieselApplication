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

        // ----------------------------------------
        // Constructor for initializing dependencies.
        // ----------------------------------------
        public DieslovaniQueryService(IDieslovaniRepository dieslovaniRepository, IRegionyService regionyService, ITechnikService technikService)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _regionyService = regionyService;
            _technikService = technikService;
        }

        // ----------------------------------------
        // Get dieslovani records by user ID.
        // ----------------------------------------
        public async Task<List<object>> GetDieslovaniByUserId(string userId)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Technik.User.Id == userId);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Get all dieslovani table data.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataAllTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery();
            query = FilteredData(query, currentUser, isEngineer);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Get running dieslovani table data.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataRunningTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup != DateTime.MinValue && i.Odchod == DateTime.MinValue);
            query = FilteredData(query, currentUser, isEngineer);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Get upcoming dieslovani table data.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataUpcomingTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID != "606794494");
            query = FilteredData(query, currentUser, isEngineer);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Get completed dieslovani table data.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataEndTableAsync(User currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.Odchod != DateTime.MinValue.Date && o.Odstavka.Do.Date <= DateTime.Today);
            query = FilteredData(query, currentUser, isEngineer);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Get trashed dieslovani table data.
        // ----------------------------------------
        public async Task<List<object>> GetTableDatathrashTableAsync(User currentUser, bool isEngineer)
        {
            var technik = await _technikService.GetTechnikByUserIdAsync(currentUser.Id);

            if (technik == null)
            {
                return new List<object>();
            }
            var firmaId = technik.Firma.ID;

            var validRegions = await _regionyService.GetRegionByIdFirmy(firmaId);

            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.Technik.ID == "606794494");

          
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Get dieslovani details for a specific odstávka.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataOdDetailOdstavkyAsync(string idodstavky)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.ID == idodstavky);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        // ----------------------------------------
        // Filter dieslovani data based on user and role.
        // ----------------------------------------
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

        // ----------------------------------------
        // Get dieslovani detail data as JSON.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataDetailJsonAsync(string id)
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

        // ----------------------------------------
        // Get dieslovani details as JSON.
        // ----------------------------------------
        public async Task<object> DetailDieslovaniJsonAsync(string id)
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
                    error = "Lokalita přiřazena k dieslovani nenalezena"
                };
            }
            else if (detail.Odstavka.Lokality.Region == null)
            {
                return new
                {
                    error = "Region přiřazen k dieslovani nenalezen"
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
                zadanVstup = detail.Vstup,
                zadanOdchod = detail.Odchod,
                Technik = detail.Technik?.User?.Id ?? "Unknown",
                jmenoTechnika = detail.Technik?.User?.Jmeno ?? "Unknown",
                prijmeniTechnika = detail.Technik?.User?.Prijmeni ?? "Unknown",
            };
        }
    }
}







