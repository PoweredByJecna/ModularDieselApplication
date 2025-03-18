using AutoMapper;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Interfaces.Repositories;
using System.Runtime.CompilerServices;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class TechniciRepository : ITechniciRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public TechniciRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------------------
        // Get Technik by ID
        // ----------------------------------------
        public async Task<Technik?> GetByIdAsync(string idTechnika)
        {
            var entity = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.ID == idTechnika);    

            return _mapper.Map<Technik?>(entity);
        }

        // ----------------------------------------
        // Get Technik by User ID
        // ----------------------------------------
        public async Task<Technik?> GetByUserIdAsync(string idUser)
        {
            var entity = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.IdUser == idUser);

            return _mapper.Map<Technik?>(entity);
        }

        // ----------------------------------------
        // Get Technik by Firma ID
        // ----------------------------------------
        public async Task<Technik> GetByFirmaIdAsync(int idFirmy)
        {
            var entity = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .Where(t => t.Taken == false)
                .FirstOrDefaultAsync(t => t.FirmaId == idFirmy);

            return _mapper.Map<Technik>(entity);
        }

        // ----------------------------------------
        // Check if Technik is on duty
        // ----------------------------------------
        public async Task<bool> IsTechnikOnDutyAsync(string idTechnika)
        {
            var technik = await _context.PohotovostiS
                .Include(p => p.Technik)
                .Where(p => p.Technik.ID == idTechnika)
                .FirstOrDefaultAsync();
            if (technik == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        // ----------------------------------------
        // Update Technik
        // ----------------------------------------
        public async Task UpdateAsync(Technik technik)
        {
            var existingEntity = await _context.TechnikS.FindAsync(technik.ID);
            if (existingEntity == null)
            {
            throw new Exception($"Technik s ID {technik.ID} nebyl nalezen.");
            }

            // Namapujte změny z doménového modelu do existující (trackované) entity.
            _mapper.Map(technik, existingEntity);
            
            // Uložte změny.
            await _context.SaveChangesAsync();
        }
        // ----------------------------------------
        // Delete Technik by ID
        // ----------------------------------------
        public async Task<bool> DeleteAsync(string idTechnika)
        {
            var entity = await _context.TechnikS.FindAsync(idTechnika);
            if (entity != null)
            {
                _context.TechnikS.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // ----------------------------------------
        // Get all Technici
        // ----------------------------------------
        public async Task<List<Technik>> GetAllAsync()
        {
            var entities = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .ToListAsync();

            return _mapper.Map<List<Technik>>(entities);
        }
    }
}