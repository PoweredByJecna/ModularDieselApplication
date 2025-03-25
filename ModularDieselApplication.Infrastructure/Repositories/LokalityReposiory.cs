using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Application.Interfaces.Repositories;
using AutoMapper;
using ModularDieselApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class LokalityRepository : ILokalityRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public LokalityRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------------------
        // Get all lokalita records.
        // ----------------------------------------
        public async Task<List<object>> GetAllLokalityAsync()
        {
            var entities = await _context.LokalityS
                .Include(l => l.Region)
                .Select(l => new
                {
                    id = l.ID,
                    nazev = l.Nazev,
                    klasifikace = l.Klasifikace,
                    adresa = l.Adresa,
                    baterie = l.Baterie,
                    zasuvka = l.Zasuvka,
                    region = l.Region.Nazev,
                    zdroj = l.Zdroj != null ? l.Zdroj.Nazev : "N/A"
                })
                .ToListAsync();

            return _mapper.Map<List<object>>(entities);
        }

        // ----------------------------------------
        // Get a lokalita record by its name.
        // ----------------------------------------
        public async Task<Lokalita> GetLokalitaByName(string nazev)
        {
            var entity = await _context.LokalityS
                .Include(l => l.Region)
                .Include(l => l.Zdroj)
                .FirstOrDefaultAsync(l => l.Nazev == nazev);

            return _mapper.Map<Lokalita>(entity);
        }

        // ----------------------------------------
        // Get detailed information about a lokalita by its name.
        // ----------------------------------------
        public async Task<Lokalita> DetailLokalityAsync(string nazev)
        {
            var entity = await _context.LokalityS
                .Include(l => l.Region)
                .Include(l => l.Zdroj)
                .Where(l => l.Nazev == nazev)
                .FirstOrDefaultAsync();

            return _mapper.Map<Lokalita>(entity);
        }

        // ----------------------------------------
        // Get dieslovani records for a specific lokalita.
        // ----------------------------------------
        public async Task<List<object>> GetDieslovaniNaLokaliteAsync(string nazev)
        {
            var entities = await _context.DieslovaniS
                .Include(d => d.Odstavka)
                .ThenInclude(d => d.Lokality)
                .ThenInclude(d => d.Region)
                .Include(d => d.Technik)
                .ThenInclude(d => d.User)
                .Where(d => d.Odstavka.Lokality.Nazev == nazev)
                .Select(l => new
                {
                    IDdieslovani = l.ID,
                    lokalitaNazev = l.Odstavka.Lokality.Nazev,
                    adresaLokality = l.Odstavka.Lokality.Adresa,
                    distrib = l.Odstavka.Distributor,
                    klasifikaceLokality = l.Odstavka.Lokality.Klasifikace,
                    vstupnaLokalitu = l.Vstup,
                    odchodzLokality = l.Odchod,
                    userId = l.Technik.User.Id ?? "Unknown",
                    jmenoTechnikaDA = l.Technik.User.Jmeno ?? "Unknown",
                    prijmeniTechnikaDA = l.Technik.User.Prijmeni ?? "Unknown",
                })
                .ToListAsync();

            return _mapper.Map<List<object>>(entities);
        }

        // ----------------------------------------
        // Get odstávky records for a specific lokalita.
        // ----------------------------------------
        public async Task<List<object>> GetOdstavkynaLokaliteAsync(string nazev)
        {
            var entities = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ThenInclude(o => o.Region)
                .Where(o => o.Lokality.Nazev == nazev)
                .Select(l => new
                {
                    odstavkaId = l.ID,
                    distributor = l.Distributor,
                    lokalita = l.Lokality.Nazev,
                    klasifikace = l.Lokality.Klasifikace,
                    adresa = l.Lokality.Adresa,
                    region = l.Lokality.Region.Nazev,
                    popis = l.Popis,
                    zacatekOdstavky = l.Od,
                    konecOdstavky = l.Do,
                    zasuvka = l.Lokality.Zasuvka,
                })
                .ToListAsync();

            return _mapper.Map<List<object>>(entities);
        }
    }
}