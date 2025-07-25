using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
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
        public async Task<Dieslovani?> GetByIdAsync(string id)
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
        public async Task<HandleResult> AddAsync(Dieslovani dieslovani)
        {
            var efEntity = _mapper.Map<TableDieslovani>(dieslovani);
            var existingOdstavka = await _context.OdstavkyS.FindAsync(efEntity.IdOdstavky);
            if (existingOdstavka == null)
            {
                return HandleResult.Error($"Odstavka s ID {efEntity.IdOdstavky} nebyla nalezena.");
            }
            efEntity.Odstavka = existingOdstavka;

            var existingTechnik = await _context.TechnikS.FindAsync(efEntity.IdTechnik);
            if (existingTechnik == null)
            {
                return HandleResult.Error($"Technik s ID {efEntity.IdTechnik} nebyl nalezen.");
            }
            efEntity.Technik = existingTechnik;
            _context.DieslovaniS.Add(efEntity);
            await _context.SaveChangesAsync();

            dieslovani.ID = efEntity.ID;

            return HandleResult.OK("Dieslovani bylo úspěšně přidáno.");
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
        public async Task<bool> DeleteAsync(string id)
        {
            var dieslovani = await _context.DieslovaniS.FindAsync(id);
            if (dieslovani != null)
            {
                var technik = dieslovani.Technik.Id;
                _context.DieslovaniS.Remove(dieslovani);
                await _context.SaveChangesAsync();

               

                var anotherDieselRequest = await AnotherDieselRequest(technik);
                if (!anotherDieselRequest)
                {
                    dieslovani.Technik.Taken = false;
                    await _context.SaveChangesAsync();
                }
                await _context.SaveChangesAsync();
                return true;
               
            }
            return false;
        }


        // ----------------------------------------
        // Get Dieslovani query
        // ----------------------------------------
        public IQueryable<Dieslovani> GetDieslovaniQuery(User? currentUser = null, bool isEngineer = false)
        {
            var query = _context.DieslovaniS
                .Include(d => d.Odstavka)
                .Include(d => d.Technik)
                .ThenInclude(d => d.User)
                .ProjectTo<Dieslovani>(_mapper.ConfigurationProvider);

            if (isEngineer)
            {
                var userId = currentUser?.Id;
                query = query.Where(d => d.Technik.User.Id == userId);
            }
            return query;
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

        // Check if another diesel request exists
        // ----------------------------------------
        public async Task<bool> AnotherDieselRequest(string idTechnika)
        {
            return await _context.DieslovaniS
                .Include(o=>o.Odstavka)
                .AnyAsync(o => o.Technik.Id == idTechnika && o.Odchod == DateTime.MinValue.Date);
        }

        // ----------------------------------------
        // Get Dieslovani with Technik by Firma ID
        // ----------------------------------------
        public async Task<Dieslovani?> GetDieslovaniWithTechnikAsync(string firmaId)
        {
            var entity = await _context.DieslovaniS
                .Include(p => p.Technik)
                .ThenInclude(P=>P.User)
                .Include(p => p.Odstavka)
                .ThenInclude(p => p.Lokality)
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
            _mapper.Map(dieslovani, existingEntity);

            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Get Dieslovani by Odstavka ID
        // ----------------------------------------
        public async Task<Dieslovani> GetDAbyOdstavkaAsync(string idOdstavky)
        {
            var entity = await _context.DieslovaniS
                .Include(o => o.Odstavka)
                .ThenInclude(o => o.Lokality)
                .ThenInclude(o => o.Region)
                .Include(o => o.Technik)
                .ThenInclude(o => o.User)
                .FirstOrDefaultAsync(o => o.IdOdstavky == idOdstavky);

            return _mapper.Map<Dieslovani>(entity);
        }

        // ----------------------------------------
        // Get the ID of an Odstavka associated with a specific Dieslovani.
        // ----------------------------------------
        public async Task<string> GetIDbyDieselId(string idDieslovani)
        {
            var entity = await _context.DieslovaniS
                .Include(d => d.Odstavka) 
                .FirstOrDefaultAsync(d => d.ID == idDieslovani); 
            if (entity == null)
            {
                throw new Exception($"Dieslovani with ID {idDieslovani} not found.");
            }    
            return entity.Odstavka.ID; 
        }

        // ----------------------------------------
        // Get an Odstavka entity by its ID.
        // ----------------------------------------
        public async Task<Odstavka> GetByOdstavkaByIdAsync(string idodstavky)
        {
            var entity = await _context.OdstavkyS
                .Include(d => d.Lokality) 
                .ThenInclude(d => d.Region) 
                .FirstOrDefaultAsync(d => d.ID == idodstavky); 

            return _mapper.Map<Odstavka>(entity);
        }

    }
}