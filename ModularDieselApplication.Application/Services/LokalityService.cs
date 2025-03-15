using System.Reflection.Metadata;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Services
{
    public class LokalityService : ILokalityService
    {
        private readonly ILokalityRepository _lokalityRepository;
        public LokalityService(ILokalityRepository lokalityRepository)
        {
            _lokalityRepository = lokalityRepository;
        }
        public async Task<List<object>> GetAllLokalityAsync()
        {
            return await _lokalityRepository.GetAllLokalityAsync();
        }
        public async Task<Lokalita> DetailLokalityAsync(string nazev)
        {
            return await _lokalityRepository.GetLokalitaByName(nazev);
        }
        public async Task<List<object>> GetDieslovaniNaLokaliteAsync(string nazev)
        {
            var detail = _lokalityRepository.GetDieslovaniNaLokaliteAsync(nazev);
            return await detail;
        }

        public async Task<List<object>> GetOdstavkynaLokaliteAsync(string nazev)
        {
            return await _lokalityRepository.GetOdstavkynaLokaliteAsync(nazev);
        }

        public async Task<object> DetailLokalityJsonAsync(string nazev)
        {
            var detialLokality =  await _lokalityRepository.DetailLokalityAsync(nazev);
            return new 
            {
                Id = detialLokality.ID,
                Lokalita = detialLokality.Nazev,
                klasifikace = detialLokality.Klasifikace,
                adresa = detialLokality.Adresa,
                baterie = detialLokality.Baterie,
                zasuvka = detialLokality.Zasuvka,
                region = detialLokality.Region.Nazev,
                zdroj = detialLokality.Zdroj?.Nazev,
            };

        }
    }
}