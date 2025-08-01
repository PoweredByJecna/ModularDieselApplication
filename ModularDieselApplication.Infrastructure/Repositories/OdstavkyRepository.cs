using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class OdstavkyRepository : IOdstavkyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public OdstavkyRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Odstavka> GetOdstavkaAsync(GetOdstavka filter, object value)
        {
            switch (filter)
            {
                case GetOdstavka.ById:
                    return await GetByIdAsync(value as string);
                default:
                    throw new InvalidDataException();
            }
        }
        // ----------------------------------------
        // Get an Odstavka record by its ID.
        // ----------------------------------------
        private async Task<Odstavka> GetByIdAsync(string id)
        {
            var entity = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ThenInclude(l => l.Region)
                .FirstOrDefaultAsync(o => o.ID == id);

            return _mapper.Map<Odstavka>(entity);
        }
        // ----------------------------------------
        // Get all Lokality records.
        // ----------------------------------------
        public async Task<List<Lokalita>> GetAllAsync()
        {
            var entities = await _context.LokalityS
                .Include(l => l.Region)
                .ToListAsync();

            return _mapper.Map<List<Lokalita>>(entities);
        }
        // ----------------------------------------
        // Add a new Odstavka record.
        // ----------------------------------------
        public async Task AddAsync(Odstavka odstavka)
        {
            var efEntity = _mapper.Map<TableOdstavka>(odstavka);

            var existingLokalita = await _context.LokalityS.FindAsync(efEntity.LokalitaID);
            if (existingLokalita == null)
            {
                throw new Exception($"Lokalita s ID {efEntity.LokalitaID} nebyla nalezena.");
            }
            efEntity.Lokality = existingLokalita;

            await _context.OdstavkyS.AddAsync(efEntity);
            await _context.SaveChangesAsync();

            odstavka.ID = efEntity.ID;
        }
        // ----------------------------------------
        // Update an existing Odstavka record.
        // ----------------------------------------
        public async Task UpdateAsync(Odstavka odstavka)
        {
            var entity = _mapper.Map<TableOdstavka>(odstavka);
            _context.OdstavkyS.Update(entity);
            await _context.SaveChangesAsync();
        }
        // ----------------------------------------
        // Get another Odstavka record by Lokalita ID and date.
        // ----------------------------------------
        public async Task<Odstavka> AnotherOdsatvkaAsync(string LokalitaId, DateTime od)
        {
            var entity = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Where(o => o.Lokality.ID == LokalitaId && o.Od == od)
                .FirstOrDefaultAsync();
            return _mapper.Map<Odstavka>(entity);
        }
        // ----------------------------------------
        // Delete an Odstavka record by its ID.
        // ----------------------------------------
        public async Task DeleteAsync(string id)
        {
            var entity = await _context.OdstavkyS.FindAsync(id);
            if (entity != null)
            {
                _context.OdstavkyS.Remove(entity);
                await _context.SaveChangesAsync();
            }
            else throw new InvalidDataException("Chyba při mazání");
        }
        // ----------------------------------------
        // Get a queryable collection of Odstavka records.
        // ----------------------------------------
        public IQueryable<Odstavka> GetOdstavkaQuery()
        {
            var entities = _context.OdstavkyS
                .Include(o => o.Lokality)
                .AsQueryable();

            return _mapper.ProjectTo<Odstavka>(entities);
        }
        
    }
}