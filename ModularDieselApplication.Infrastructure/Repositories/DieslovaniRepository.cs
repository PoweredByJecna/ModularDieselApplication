using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class DieslovaniRepository : RepositoriesBaseClass, IDieslovaniRepository
    {
        public DieslovaniRepository(ApplicationDbContext context, IMapper mapper) : base(context, mapper)
        {
        }

        public async Task<Dieslovani> GetDaAsync(GetDA filter, object value)
        {
            switch (filter)
            {
                case GetDA.ById:
                    return await GetByIdAsync(value as string);
                case GetDA.ByOdstavkaId:
                    return await GetDAbyOdstavkaAsync(value as string);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        // ----------------------------------------
        // Get Dieslovani by ID
        // ---------------------------------------- 
        private async Task<Dieslovani> GetByIdAsync(string id)
        {
            var entity = await _context.DieslovaniS
                .Include(d => d.Odstavka)
                .ThenInclude(d => d.Lokality)
                .ThenInclude(d => d.Region)
                .Include(d => d.Technik)
                .ThenInclude(d => d.User)
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

        // Check if another diesel request exists
        // ----------------------------------------
        public async Task<bool> AnotherDieselRequest(string idTechnika)
        {
            return await _context.DieslovaniS
                .Include(o => o.Odstavka)
                .AnyAsync(o => o.Technik.Id == idTechnika && o.Odchod == DateTime.MinValue.Date);
        }

        // ----------------------------------------
        // Get Dieslovani by Odstavka ID
        // ----------------------------------------
        private async Task<Dieslovani> GetDAbyOdstavkaAsync(string idOdstavky)
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
        public async Task<List<Dieslovani>> GetAnotherDA(Dieslovani dieslovani)
        { 
            var anotherDieslovani = await _context.DieslovaniS
                .Include(d => d.Odstavka)
                .ThenInclude(d => d.Lokality)
                .ThenInclude(d => d.Region)
                .Include(d => d.Technik)
                .Where(d=> d.ID != dieslovani.ID && d.Odstavka.Lokality.Region.ID == dieslovani.Odstavka.Lokality.Region.ID )
                .ToListAsync();
            return _mapper.Map<List<Dieslovani>>(anotherDieslovani);
        }

    }
}