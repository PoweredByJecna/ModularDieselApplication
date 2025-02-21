using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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
        public async Task<int> GetLokalitaCountAsync()
        {
            return await _context.LokalityS.CountAsync();
        }

        // ----------------------------------------
        // Get Odstavka by ID
        // ----------------------------------------
        public async Task<Odstavka?> GetByIdAsync(int id)
        {
            var entity = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ThenInclude(l => l.Region)
                .FirstOrDefaultAsync(o => o.ID == id);

            return _mapper.Map<Odstavka?>(entity);
        }

        // ----------------------------------------
        // Get all Odstavka
        // ----------------------------------------
        public async Task<List<Lokalita>> GetAllAsync()
        {
            var entities = await _context.LokalityS
                .Include(l => l.Region)
                .ToListAsync();

            return _mapper.Map<List<Lokalita>>(entities);
        }

        // ----------------------------------------
        // Add new Odstavka
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
        public async Task<bool> DeleteAsync(int id)
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
        public async Task<Lokalita?> GetByNameAsync(string name)
        {
            var lokalita = await _context.LokalityS
                .Include(l=>l.Region)
                .FirstOrDefaultAsync(l => l.Nazev == name);
            return lokalita != null ? _mapper.Map<Lokalita>(lokalita) : null;
        }


        // ----------------------------------------
        // Get Lokality by ID
        // ----------------------------------------
        public async Task<Lokalita> GetLokalityByIdAsync(int id)
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
        // Get Odstavka query
        // ----------------------------------------
        public IQueryable<Odstavka> GetOdstavkaQuery()
        {
            var entities = _context.OdstavkyS
                .Include(o => o.Lokality)
                .AsQueryable();

            return _mapper.ProjectTo<Odstavka>(entities);
        }

        // ----------------------------------------
        // Get Odstavka data
        // ----------------------------------------
        public async Task<List<object>> GetOdstavkaDataAsync(IQueryable<Odstavka> query)
        {
            var odstavkaList = await _context.OdstavkyS
                .Include(l => l.Lokality)
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
                    Dieslovani = _context.DieslovaniS.FirstOrDefault(d => d.IDodstavky == l.ID)
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
                IdTechnika = l.Dieslovani?.Technik?.ID,
                ZadanVstup = l.Dieslovani?.Vstup,
                ZadanOdchod = l.Dieslovani?.Odchod
            }).ToList();

            return _mapper.Map<List<object>>(result);
        }
        private async Task<Dieslovani?> GetDieslovanis(int idodstavky)
        {
            var dieslovani = await _context.DieslovaniS
                .Where(d => d.IDodstavky == idodstavky)
                .FirstOrDefaultAsync();
                return dieslovani != null ? _mapper.Map<Dieslovani>(dieslovani) : null;
        }        
    }
}