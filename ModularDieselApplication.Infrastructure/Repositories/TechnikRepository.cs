using AutoMapper;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using ModularDieselApplication.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using ModularDieselApplication.Interfaces.Repositories;

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
        // Get Technik by ID.
        // ----------------------------------------
        public async Task<Technik?> GetByIdAsync(string idTechnika)
        {
            var entity = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Id == idTechnika);

            return _mapper.Map<Technik?>(entity);
        }

        // ----------------------------------------
        // Get Technik by User ID.
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
        // Get Technik by Firma ID.
        // ----------------------------------------
        public async Task<Technik> GetByFirmaIdAsync(string idFirmy)
        {
            var entity = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .Where(t => t.Taken == false)
                .FirstOrDefaultAsync(t => t.FirmaId == idFirmy);

            return _mapper.Map<Technik>(entity);
        }

        // ----------------------------------------
        // Check if a Technik is currently on duty.
        // ----------------------------------------
        public async Task<bool> IsTechnikOnDutyAsync(string idTechnika)
        {
            var technik = await _context.PohotovostiS
                .Include(p => p.Technik)
                .Where(p => p.Technik.Id == idTechnika)
                .FirstOrDefaultAsync();

            return technik != null;
        }

        // ----------------------------------------
        // Update an existing Technik.
        // ----------------------------------------
        public async Task UpdateAsync(Technik technik)
        {
            var existingEntity = await _context.TechnikS.FindAsync(technik.ID);
            if (existingEntity == null)
            {
                throw new Exception($"Technik with ID {technik.ID} not found.");
            }

            // Map changes from the domain model to the tracked entity.
            _mapper.Map(technik, existingEntity);

            // Save changes to the database.
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Delete a Technik by ID.
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
        // Get all Technici records.
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