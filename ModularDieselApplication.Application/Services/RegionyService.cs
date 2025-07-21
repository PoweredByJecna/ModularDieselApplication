using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Enum;

namespace ModularDieselApplication.Application.Services
{
    public class RegionyService : IRegionyService
    {
        private readonly IRegionyRepository _regionyRepository;

        public RegionyService(IRegionyRepository regionyRepository)
        {
            _regionyRepository = regionyRepository;
        }

        // ----------------------------------------
        // Get region data by firm ID.
        // ----------------------------------------
        public async Task<List<object>> GetRegionByIdFirmy(string _IdRegionu)
        {
            var resultList = await GetDataRegion(_IdRegionu);
            return resultList;
        }

        // ----------------------------------------
        // Get firm data for a specific region.
        // ----------------------------------------
        public async Task<Firma> GetFirmaVRegionuAsync(string idReg)
        {
            var firma = await _regionyRepository.GetFirmaAsync(idReg);
            return firma;
        }

        // ----------------------------------------
        // Get detailed region data by region Name.
        // ----------------------------------------
        private async Task<List<object>> GetDataRegion(string name)
        {
            var regiony = await _regionyRepository.GetRegionByName(name);
            var (pocetLokalit, pocetOdstavek) = await _regionyRepository.GetCountAsync(name);

            List<object> resultList = new();

            foreach (var reg in regiony)
            {
                var technikList = await _regionyRepository.GetTechnikListVRegionu(reg.Firma.ID);
                var techniciDto = technikList.Select(t => new
                {
                    jmeno = $"{t.User.Jmeno} {t.User.Prijmeni}",
                    userId = t.User.Id,
                    maPohotovost = _regionyRepository.GetValueIfTechnikHasPohotovost(t.ID)
                }).ToList();

                var regionData = new
                {
                    firma = reg.Firma.Nazev,
                    distributor = DetermineDistributor(reg.Nazev),
                    pocetLokalit,
                    pocetOdstavek,
                    technici = techniciDto
                };

                resultList.Add(regionData);
            }
            return resultList;
        }

        // ----------------------------------------
        // Determine the distributor based on the region name.
        // ----------------------------------------
        private string DetermineDistributor(string NazevRegionu)
        {
            return NazevRegionu switch
            {
                "Severní Čechy" or "Západní Čechy" or "Severní Morava" => "ČEZ",
                "Jižní Morava" or "Jižní Čechy" => "EGD",
                "Praha + Střední Čechy" => "PRE",
                _ => ""
            };
        }
        
        public async Task<List<object>> GetRegionData(RegionyFilterEnum regionFilter)
        {
            return regionFilter switch
            {
                RegionyFilterEnum.Praha => await GetDataRegion("Praha + Střední Čechy"),
                RegionyFilterEnum.SeverniMorava => await GetDataRegion("Severní Morava"),
                RegionyFilterEnum.JizniMorava => await GetDataRegion("Jižní Morava"),
                RegionyFilterEnum.ZapadniCechy => await GetDataRegion("Západní Čechy"),
                RegionyFilterEnum.SeverniCechy => await GetDataRegion("Severní Čechy"),
                RegionyFilterEnum.JizniCechy => await GetDataRegion("Jižní Čechy"),
                _ => new List<object>(),
            };
        }
    }
}