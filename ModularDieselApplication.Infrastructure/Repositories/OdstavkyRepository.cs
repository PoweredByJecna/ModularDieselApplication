using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Entities;
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

        // ----------------------------------------
        // Get the count of Lokality records.
        // ----------------------------------------
        public async Task<int> GetLokalitaCountAsync()
        {
            return await _context.LokalityS.CountAsync();
        }

        // ----------------------------------------
        // Get an Odstavka record by its ID.
        // ----------------------------------------
        public async Task<Odstavka> GetByIdAsync(string id)
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
            var efEntity = _mapper.Map<TableOdstavky>(odstavka);

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
            var entity = _mapper.Map<TableOdstavky>(odstavka);
            _context.OdstavkyS.Update(entity);
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Get another Odstavka record by Lokalita ID and date.
        // ----------------------------------------
        public async Task<Odstavka?> AnotherOdsatvkaAsync(string LokalitaId, DateTime od)
        {
            var entity = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Where(o => o.Lokality.ID == LokalitaId && o.Od == od)
                .FirstOrDefaultAsync();

            return _mapper.Map<Odstavka?>(entity);
        }

        // ----------------------------------------
        // Delete an Odstavka record by its ID.
        // ----------------------------------------
        public async Task<bool> DeleteAsync(string id)
        {
            var entity = await _context.OdstavkyS.FindAsync(id);
            if (entity != null)
            {
                _context.OdstavkyS.Remove(entity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        // ----------------------------------------
        // Check if another diesel request exists for a technician.
        // ----------------------------------------
        public async Task<bool> AnotherDieselRequest(string idTechnika)
        {
            return await _context.DieslovaniS
                .AnyAsync(d => d.Technik.Id == idTechnika);
        }

        // ----------------------------------------
        // Get a Lokalita record by its name.
        // ----------------------------------------
        public async Task<Lokalita?> GetByNameAsync(string name)
        {
            var lokalita = await _context.LokalityS
                .Include(l => l.Region)
                .FirstOrDefaultAsync(l => l.Nazev == name);
            return lokalita != null ? _mapper.Map<Lokalita>(lokalita) : null;
        }

        // ----------------------------------------
        // Get a Lokalita record by its ID.
        // ----------------------------------------
        public async Task<Lokalita> GetLokalityByIdAsync(string id)
        {
            var lokalitaEntity = await _context.LokalityS
                .Include(l => l.Region)
                .FirstOrDefaultAsync(l => l.ID == id);

            if (lokalitaEntity == null)
            {
                throw new Exception($"Lokalita with ID {id} not found.");
            }
            return _mapper.Map<Lokalita>(lokalitaEntity);
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

        // ----------------------------------------
        // Get Odstavka data based on a query.
        // ----------------------------------------
        public async Task<List<object>> GetOdstavkaDataAsync(IQueryable<Odstavka> query)
        {
            var odstavkaList = await query
                .Select(l => new
                {
                    id = l.ID,
                    Distributor = l.Distributor,
                    NazevLokality = l.Lokality.Nazev,
                    Klasifikace = l.Lokality.Klasifikace,
                    ZacatekOdstavky = l.Od,
                    KonecOdstavky = l.Do,
                    Adresa = l.Lokality.Adresa,
                    VydrzBaterie = l.Lokality.Baterie,
                    Popis = l.Popis,
                    Zasuvka = l.Lokality.Zasuvka,
                    Dieslovani = _context.DieslovaniS.Include(o => o.Technik).FirstOrDefault(d => d.IDodstavky == l.ID)
                })
                .ToListAsync();

            var result = odstavkaList.Select(l => new
            {
                l.id,
                l.Distributor,
                l.NazevLokality,
                l.Klasifikace,
                l.ZacatekOdstavky,
                l.KonecOdstavky,
                l.Adresa,
                l.VydrzBaterie,
                l.Popis,
                l.Zasuvka,
                idTechnika = l.Dieslovani?.Technik?.Id,
                zadanVstup = l.Dieslovani?.Vstup,
                zadanOdchod = l.Dieslovani?.Odchod
            }).ToList();

            return _mapper.Map<List<object>>(result);
        }
    }
}