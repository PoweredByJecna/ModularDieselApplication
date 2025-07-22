using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Objects;
using Microsoft.EntityFrameworkCore;

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
        public async Task<List<Dieslovani>> GetDieslovaniByUserId(string userId)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Technik.User.Id == userId);
            return await query.ToListAsync();
        }

        // ----------------------------------------
        // Get dieslovani details for a specific odstávka.
        // ----------------------------------------
        public async Task<List<Dieslovani>> GetTableDataOdDetailOdstavkyAsync(string idodstavky)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.ID == idodstavky);
            return await query.ToListAsync();
        }

        // ----------------------------------------
        // Filter dieslovani data based on user and role.
        // ----------------------------------------
        private static IQueryable<Dieslovani> FilteredData(IQueryable<Dieslovani> query, User currentUser, bool isEngineer)
        {
         
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
        public async Task<HandleResult<Dieslovani>> DetailDieslovaniJsonAsync(string id)
        {
            Dieslovani? detail = await _dieslovaniRepository.GetByIdAsync(id);

            return detail switch
            {
                null => HandleResult.Error("Dieslovani nenalezeno"),
                { Odstavka: null } => HandleResult.Error("Odstavka přiřazena k dieslovani nenalezena"),
                { Odstavka.Lokality: null } => HandleResult.Error("Lokalita přiřazena k dieslovani nenalezena"),
                { Odstavka.Lokality.Region: null } => HandleResult.Error("Region přiřazen k dieslovani nenalezen"),
                _ => detail
            };
        }
    }
}








