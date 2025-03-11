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
        public async Task<Lokalita> DetailLokalityAsync(int id)
        {
            return await _lokalityRepository.GeLokalitaByID(id);
        }
        public async Task<object> DetailLokalityJsonAsync(int id)
        {
            var detialLokality =  await _lokalityRepository.DetailLokalityAsync(id);
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