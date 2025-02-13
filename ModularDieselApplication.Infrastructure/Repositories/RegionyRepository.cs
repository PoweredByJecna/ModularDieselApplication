using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;
using Org.BouncyCastle.Asn1;
using System.Collections.Generic;
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
        // Get data by region ID
        // ----------------------------------------
        public async Task<List<object>> GetData(int regionId)
        {
            var regions = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.ID == regionId)
                .ToListAsync();

            var resultList = _mapper.Map<List<object>>(regions);
            return resultList;
        }
            public async Task<Firma> GetFirmaAsync(int idReg)
            {
                var firma = await _context.RegionS
                    .Include(r => r.Firma)
                    .Where(r => r.ID == idReg)
                    .Select(r => r.Firma)
                    .FirstOrDefaultAsync();
                return _mapper.Map<Firma>(firma);
            }


        // ----------------------------------------
        // Check if Technik has Pohotovost
        // ----------------------------------------
        public async Task<bool> TechnikHasPohotovost(string idTechnik)
        {
            var exists = await _context.PohotovostiS
                .AnyAsync(p => p.IdTechnik == idTechnik);
            return exists;
        }

        // ----------------------------------------
        // Get regions by Firma ID
        // ----------------------------------------
        public async Task<List<object>> GetRegion(int firmaId)
        {
            var regions = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.FirmaID == firmaId)
                .ToListAsync();

            var resultList = _mapper.Map<List<object>>(regions);
            return resultList;
        }
    }
}