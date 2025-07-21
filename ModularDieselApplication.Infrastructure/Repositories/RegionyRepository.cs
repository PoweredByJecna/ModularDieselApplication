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
        // Get the count of Lokality records in a region.
        // ----------------------------------------
        public async Task<(int pocetLokalit, int pocetOdstavek)> GetCountAsync(string name)
        {
            var pocetlokalit = await _context.LokalityS
                .Include(o => o.Region)
                .Where(o => o.Region.Nazev == name)
                .CountAsync();

            var pocetOdstavek = await _context.OdstavkyS
                .Include(o => o.Lokality)
                .ThenInclude(o => o.Region)
                .Where(o => o.Lokality.Region.Nazev == name)
                .CountAsync();
            return (pocetlokalit, pocetOdstavek);
        }
        // ----------------------------------------
        // Get region data by its Name.
        // ----------------------------------------
        public async Task<List<Region>> GetRegionByName(string name)
        {
            var region = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.Nazev == name)
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