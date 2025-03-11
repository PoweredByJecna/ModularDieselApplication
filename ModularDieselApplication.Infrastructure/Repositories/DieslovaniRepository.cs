using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class DieslovaniRepository : IDieslovaniRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public DieslovaniRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------------------
        // Get Dieslovani by ID
        // ----------------------------------------
        public async Task<Dieslovani?> GetByIdAsync(int id)
        {
            var entity = await _context.DieslovaniS
                .Include(d => d.Odstavka)
                .ThenInclude(d=>d.Lokality)
                .ThenInclude(d=>d.Region)
                .Include(d => d.Technik)
                .ThenInclude(d=>d.User)
                .FirstOrDefaultAsync(d => d.ID == id);

            return entity == null ? null : _mapper.Map<Dieslovani>(entity);
        }

        // ----------------------------------------
        // Add new Dieslovani
        // ----------------------------------------
        

        public async Task AddAsync(Dieslovani dieslovani)
        {
            var efEntity = _mapper.Map<TableDieslovani>(dieslovani);
            var existingOdstavka = await _context.OdstavkyS.FindAsync(efEntity.IDodstavky);
            if (existingOdstavka == null)
            {
                throw new Exception($"Odstavka s ID {efEntity.IDodstavky} nebyla nalezena.");
            }
            efEntity.Odstavka = existingOdstavka;

            var existingTechnik = await _context.TechnikS.FindAsync(efEntity.IdTechnik);
            if (existingTechnik == null)
            {
                throw new Exception($"Technik s ID {efEntity.IdTechnik} nebyl nalezen.");
            }
            efEntity.Technik = existingTechnik;
            _context.DieslovaniS.Add(efEntity);
            await _context.SaveChangesAsync();

            dieslovani.ID = efEntity.ID;
        }

        // ----------------------------------------
        // Get Technik by ID
        // ----------------------------------------
        public async Task<Dieslovani?> GetTechnikByIdAsync(string technikId)
        {
            var entity = await _context.TechnikS
                .FirstOrDefaultAsync(p => p.ID == technikId);

            return entity == null ? null : _mapper.Map<Dieslovani>(entity);
        }

        // ----------------------------------------
        // Update Dieslovani
        // ----------------------------------------
        public async Task UpdateAsync(Dieslovani dieslovani)
        {
            var existingEntity = await _context.DieslovaniS.FindAsync(dieslovani.ID);
            if (existingEntity == null)
            {
            throw new Exception($"Záznam s ID {dieslovani.ID} neexistuje.");
            }

            _mapper.Map(dieslovani, existingEntity);


            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Delete Dieslovani by ID
        // ----------------------------------------
        public async Task<bool> DeleteAsync(int id)
        {
            var dieslovani = await _context.DieslovaniS.FindAsync(id);
            if (dieslovani != null)
            {
                _context.DieslovaniS.Remove(dieslovani);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // ----------------------------------------
        // Get count of Dieslovani
        // ----------------------------------------
        public async Task<int> GetCountAsync()
        {
            return await _context.DieslovaniS.CountAsync();
        }

        // ----------------------------------------
        // Get Dieslovani query
        // ----------------------------------------
        public IQueryable<Dieslovani> GetDieslovaniQuery()
        {
            return _context.DieslovaniS
                .Include(d => d.Odstavka)
                .Include(d => d.Technik)
                .ProjectTo<Dieslovani>(_mapper.ConfigurationProvider);
        }

        // ----------------------------------------
        // Get Dieslovani data
        // ----------------------------------------
        public async Task<List<object>> GetDieslovaniDataAsync(IQueryable<Dieslovani> query)
        {
            var dieslovanList = await query
                .OrderBy(o => o.Odstavka.Od)
                .Select(l => new
                {
                    id = l.ID,
                    distributor = l.Odstavka.Distributor,
                    lokalitaNazev = l.Odstavka.Lokality.Nazev,
                    klasifikace = l.Odstavka.Lokality.Klasifikace,
                    adresa = l.Odstavka.Lokality.Adresa,
                    technikFirma = l.Technik.Firma.Nazev,
                    jmenoTechnika = l.Technik.User.Jmeno,
                    prijmeniTechnika = l.Technik.User.Prijmeni,
                    zadanVstup = l.Vstup,
                    zadanOdchod = l.Odchod,
                    Idtechnika = l.Technik.ID,
                    NazevRegionu = l.Odstavka.Lokality.Region.Nazev,
                    OdstavkaZacatek = l.Odstavka.Od,
                    OdstavkaKonec = l.Odstavka.Do,
                    Popis = l.Odstavka.Popis,
                    VydrzBaterie = l.Odstavka.Lokality.Baterie,
                    Zasuvka = l.Odstavka.Lokality.Zasuvka,
                    User = l.Technik.User.Id
                })
                .ToListAsync();
            return dieslovanList.Cast<object>().ToList();
        }

        // ----------------------------------------
        // Check if another diesel request exists
        // ----------------------------------------
        public async Task<bool> AnotherDieselRequest(string idTechnika)
        {
            return await _context.DieslovaniS
                .AnyAsync(d => d.Technik.ID == idTechnika);
        }

        // ----------------------------------------
        // Get Dieslovani with Technik by Firma ID
        // ----------------------------------------
        public async Task<Dieslovani?> GetDieslovaniWithTechnikAsync(int firmaId)
        {
            var entity = await _context.DieslovaniS
                .Include(p => p.Technik)
                .Where(p => p.Technik.Firma.ID == firmaId && p.Technik.Taken == true)
                .FirstOrDefaultAsync();

            return _mapper.Map<Dieslovani?>(entity);
        }

        // ----------------------------------------
        // Update Dieslovani
        // ----------------------------------------
        public async Task UpdateDieslovaniAsync(Dieslovani dieslovani)
        {
            var existingEntity = await _context.DieslovaniS.FindAsync(dieslovani.ID);
            if (existingEntity == null)
            {
            throw new Exception($"Technik s ID {dieslovani.ID} nebyl nalezen.");
            }

            // Namapujte změny z doménového modelu do existující (trackované) entity.
            _mapper.Map(dieslovani, existingEntity);

            // Uložte změny.
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Get Dieslovani by Odstavka ID
        // ----------------------------------------
        public async Task<Dieslovani> GetDAbyOdstavkaAsync(int idOdstavky)
        {
            var entity = await _context.DieslovaniS
                .Include(o => o.Odstavka)
                .Include(o => o.Technik)
                .FirstOrDefaultAsync(o => o.Odstavka.ID == idOdstavky);

            return _mapper.Map<Dieslovani>(entity);
        }

        public async Task<int> CountAsync(IQueryable<Dieslovani> query)
        {
            return await query.CountAsync();
        }
    }
}