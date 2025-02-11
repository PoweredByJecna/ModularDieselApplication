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
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }
        
        public async Task<List<object>> GetRegionDataPrahaAsync()
        {
            var _IdRegionu = 4;
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataSeverniMoravaAsync()
        {
            var _IdRegionu = 3;
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataJizniMoravaAsync()
        {
            var _IdRegionu = 2;
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataJizniCechyAsync()
        {
            var _IdRegionu = 5;
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataSeverniCechyAsync()
        {
            var _IdRegionu = 1;
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }

        public async Task<List<object>> GetRegionDataZapadniCechyAsync()
        {
            var _IdRegionu = 6;
            var resultList = await _regionyRepository.GetData(_IdRegionu);
            return resultList;
        }
    }
}