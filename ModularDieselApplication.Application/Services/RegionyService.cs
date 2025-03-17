using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;

namespace ModularDieselApplication.Application.Services
{
    public class RegionyService : IRegionyService
    {
        private readonly IRegionyRepository _regionyRepository;
    
        public RegionyService(IRegionyRepository regionyRepository)
        {
            _regionyRepository = regionyRepository;
        }
 
        public async Task<List<object>> GetRegionByIdFirmy(int _IdRegionu)
        {
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }
        
        public async Task<List<object>> GetRegionDataPrahaAsync()
        {
            var _IdRegionu = 4;
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataSeverniMoravaAsync()
        {
            var _IdRegionu = 3;
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataJizniMoravaAsync()
        {
            var _IdRegionu = 2;
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataJizniCechyAsync()
        {
            var _IdRegionu = 5;
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataSeverniCechyAsync()
        {
            var _IdRegionu = 1;
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataZapadniCechyAsync()
        {
            var _IdRegionu = 6;
            var resultList = await GetDataRegioon(_IdRegionu);
            return resultList;
        }

        public async Task<Firma> GetFirmaVRegionuAsync(int idReg)
        {
            var firma = await _regionyRepository.GetFirmaAsync(idReg);
            return firma;
        }
        private async Task<List<object>> GetDataRegioon(int regionId)
        {
            var regiony = await _regionyRepository.GetRegionById(regionId);
            var pocetLokalit = await _regionyRepository.GetLokalityCountAsync(regionId);
            var pocetOdstavek = await _regionyRepository.GetOdstavkyCountAsync(regionId);

            var resultList = new List<object>();

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
                    distributor =DetermineDistributor(reg.Nazev),
                    pocetLokalit,
                    pocetOdstavek,
                    technici = techniciDto
                };

                resultList.Add(regionData);
            }
            return resultList;
        }
        private  string DetermineDistributor(string NazevRegionu)
        {
            return NazevRegionu switch
            {
                "Severní Čechy" or "Západní Čechy" or "Severní Morava" => "ČEZ",
                "Jižní Morava" or "Jižní Čechy" => "EGD",
                "Praha + Střední Čechy" => "PRE",
                _ => ""
            };
        }
    }
}