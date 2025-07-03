using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class RegionyRepository : IRegionyRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public RegionyRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------------------
        // Get data by region ID.
        // ----------------------------------------
        public async Task<List<object>> GetData(string regionId)
        {
            var regions = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.ID == regionId)
                .ToListAsync();

            var resultList = _mapper.Map<List<object>>(regions);
            return resultList;
        }

        // ----------------------------------------
        // Get Firma data by region ID.
        // ----------------------------------------
        public async Task<Firma> GetFirmaAsync(string idReg)
        {
            var firma = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.ID == idReg)
                .Select(r => r.Firma)
                .FirstOrDefaultAsync();
            return _mapper.Map<Firma>(firma);
        }

        // ----------------------------------------
        // Check if a Technik has Pohotovost.
        // ----------------------------------------
        public async Task<bool> TechnikHasPohotovost(string idTechnik)
        {
            var exists = await _context.PohotovostiS
                .AnyAsync(p => p.IdTechnik == idTechnik);
            return exists;
        }

        // ----------------------------------------
        // Get regions by Firma ID.
        // ----------------------------------------
        public async Task<List<object>> GetRegion(string firmaId)
        {
            var regions = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.FirmaID == firmaId)
                .ToListAsync();

            var resultList = _mapper.Map<List<object>>(regions);
            return resultList;
        }

        // ----------------------------------------
        // Get the count of Lokality records in a region.
        // ----------------------------------------
        public async Task<int> GetLokalityCountAsync(string regionId)
        {
            return await _context.LokalityS
                .Include(o => o.Region)
                .Where(o => o.Region.ID == regionId)
                .CountAsync();
        }

        // ----------------------------------------
        // Get the count of Odstavky records in a region.
        // ----------------------------------------
        public async Task<int> GetOdstavkyCountAsync(string regionId)
        {
            return await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ThenInclude(o => o.Region)
                .Where(o => o.Lokality.Region.ID == regionId)
                .CountAsync();
        }

        // ----------------------------------------
        // Get region data by its ID.
        // ----------------------------------------
        public async Task<List<Region>> GetRegionById(string idregion)
        {
            var region = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.ID == idregion)
                .ToListAsync();
            var resultList = _mapper.Map<List<Region>>(region);
            return resultList;
        }

        // ----------------------------------------
        // Check if a Technik has an active Pohotovost.
        // ----------------------------------------
        public bool GetValueIfTechnikHasPohotovost(string idTechnik)
        {
            var exists = _context.PohotovostiS
                .Any(p => p.IdTechnik == idTechnik && p.Konec.Date >= DateTime.Today);
            return exists;
        }

        // ----------------------------------------
        // Get a list of Techniks in a specific Firma.
        // ----------------------------------------
        public async Task<List<Technik>> GetTechnikListVRegionu(string IDfirmy)
        {
            var technik = await _context.TechnikS
                .Include(t => t.Firma)
                .Include(t => t.User)
                .Where(t => t.Firma.ID == IDfirmy)
                .ToListAsync();
            var resultList = _mapper.Map<List<Technik>>(technik);
            return resultList;
        }
    }
}