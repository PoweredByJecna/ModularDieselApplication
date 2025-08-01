using System.Reflection.Metadata;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;


namespace ModularDieselApplication.Application.Services
{
    public class LokalityService : ILokalityService
    {
        private readonly ILokalityRepository _lokalityRepository;

        public LokalityService(ILokalityRepository lokalityRepository)
        {
            _lokalityRepository = lokalityRepository;
        }

        public async Task<Lokalita> GetLokalita(GetLokalita filter, object value)
        {
            return await _lokalityRepository.GetLokalitaAsync(filter, value);
        }


        // ----------------------------------------
        // Get all lokalita records.
        // ----------------------------------------
        public async Task<List<Lokalita>> GetAllLokalityAsync()
        {
            return await _lokalityRepository.GetAllLokalityAsync();
        }

        // ----------------------------------------
        // Get dieslovani records for a specific lokalita.
        // ----------------------------------------
        public async Task<List<Dieslovani>> GetDieslovaniNaLokaliteAsync(string nazev)
        {
            var detail = _lokalityRepository.GetDieslovaniNaLokaliteAsync(nazev);
            return await detail;
        }

        // ----------------------------------------
        // Get odstávky records for a specific lokalita.
        // ----------------------------------------
        public async Task<List<Odstavka>> GetOdstavkynaLokaliteAsync(string nazev)
        {
            return await _lokalityRepository.GetOdstavkynaLokaliteAsync(nazev);
        }
    }
}