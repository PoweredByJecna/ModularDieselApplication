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
        // Get count of Odstavka
        // ----------------------------------------
        public async Task<int> GetOdstavkaCountAsync()
        {
            return await _context.PohotovostiS.CountAsync();
        }

        // ----------------------------------------
        // Get Odstavka by ID
        // ----------------------------------------
        public async Task<Odstavka?> GetByIdAsync(int id)
        {
            var entity = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Include(o => o.DieslovaniList)
                .FirstOrDefaultAsync(o => o.ID == id);

            return _mapper.Map<Odstavka?>(entity);
        }

        // ----------------------------------------
        // Get all Odstavka
        // ----------------------------------------
        public async Task<List<Odstavka>> GetAllAsync()
        {
            var entities = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Include(o => o.DieslovaniList)
                .ToListAsync();

            return _mapper.Map<List<Odstavka>>(entities);
        }

        // ----------------------------------------
        // Add new Odstavka
        // ----------------------------------------
        public async Task AddAsync(Odstavka odstavka)
        {
            var entity = _mapper.Map<TableOdstavky>(odstavka);
            _context.OdstavkyS.Add(entity);
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Update Odstavka
        // ----------------------------------------
        public async Task UpdateAsync(Odstavka odstavka)
        {
            var entity = _mapper.Map<TableOdstavky>(odstavka);
            _context.OdstavkyS.Update(entity);
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Get another Odstavka by Lokalita ID and date
        // ----------------------------------------
        public async Task<Odstavka?> AnotherOdsatvkaAsync(int LokalitaId, DateTime od)
        {
            var entity = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .Where(o => o.Lokality.ID == LokalitaId && o.Od == od)
                .FirstOrDefaultAsync();

            return _mapper.Map<Odstavka?>(entity);
        }

        // ----------------------------------------
        // Delete Odstavka by ID
        // ----------------------------------------
        public async Task DeleteAsync(int id)
        {
            var entity = await _context.OdstavkyS.FindAsync(id);
            if (entity != null)
            {
                _context.OdstavkyS.Remove(entity);
                await _context.SaveChangesAsync();
            }
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
        // Get Lokalita by name
        // ----------------------------------------
        public Task<Lokalita?> GetByNameAsync(string name)
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------
        // Get Odstavka by Lokalita name
        // ----------------------------------------
        public Task<List<Odstavka>> GetByLokalitaAsync(string name)
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------
        // Get Lokality by ID
        // ----------------------------------------
        public Task<Lokalita> GetLokalityByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        // ----------------------------------------
        // Get Odstavka query
        // ----------------------------------------
        public IQueryable<Odstavka> GetOdstavkaQuery()
        {
            var entities = _context.OdstavkyS
                .Include(o => o.Lokality)
                .Include(o => o.DieslovaniList)
                .AsQueryable();

            return _mapper.ProjectTo<Odstavka>(entities);
        }

        // ----------------------------------------
        // Get Odstavka data
        // ----------------------------------------
        public async Task<List<object>> GetOdstavkaDataAsync(IQueryable<Odstavka> query)
        {
            var odstavkaList = await query
                .Select(l => new
                {
                    l.ID,
                    l.Distributor,
                    NazevLokality = l.Lokality.Nazev,
                    Klasifikace = l.Lokality.Klasifikace,
                    l.Od,
                    l.Do,
                    Adresa = l.Lokality.Adresa,
                    Baterie = l.Lokality.Baterie,
                    l.Popis,
                    Zasuvka = l.Lokality.Zasuvka,
                    IdTechnika = _context.DieslovaniS
                        .Where(d => d.IDodstavky == l.ID)
                        .Select(d => d.Technik.ID)
                        .FirstOrDefault()
                })
                .ToListAsync();

            return _mapper.Map<List<object>>(odstavkaList);
        }
    }
}