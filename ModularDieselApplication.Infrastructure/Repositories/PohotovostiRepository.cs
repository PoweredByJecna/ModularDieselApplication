using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class PohotovostiRepository : IPohotovostiRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public PohotovostiRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------------------
        // Get Pohotovost by ID
        // ----------------------------------------
        public async Task<Pohotovosti> GetPohotovostByIdAsync(int id)
        {
            var tablePohotovosti = await _context.PohotovostiS
                .Include(o => o.Technik)
                .FirstOrDefaultAsync(o => o.ID == id);
            return _mapper.Map<Pohotovosti>(tablePohotovosti);
        }

        // ----------------------------------------
        // Check if Pohotovosti exists in a region
        // ----------------------------------------
        public async Task<bool> GetPohotovostiRegionAsync(int id)
        {
            var tablePohotovosti = await _context.PohotovostiS
                .Include(o => o.Technik)
                .ThenInclude(o => o.Firma)
                .Where(o => o.Technik != null && o.Technik.Firma != null && o.Technik.Firma.ID == id)
                .FirstOrDefaultAsync();
                if (tablePohotovosti == null)
                {
                    return false;
                }
                else return true;
        }

        // ----------------------------------------
        // Get all Pohotovosti
        // ----------------------------------------
        public async Task<List<Pohotovosti>> GetAllPohotovostiAsync()
        {
            var tablePohotovosti = await _context.PohotovostiS
                .Include(o => o.Technik)
                .ToListAsync();
            return _mapper.Map<List<Pohotovosti>>(tablePohotovosti);
        }

        // ----------------------------------------
        // Add new Pohotovost
        // ----------------------------------------
        public async Task AddPohotovostAsync(Pohotovosti pohotovosti)
        {
            var tablePohotovosti = _mapper.Map<TablePohotovosti>(pohotovosti);
            _context.PohotovostiS.Add(tablePohotovosti);
            await _context.SaveChangesAsync();
        }

        // ----------------------------------------
        // Get count of Pohotovost
        // ----------------------------------------
        public async Task<int> GetPohotovostCountAsync()
        {
            return await _context.PohotovostiS.CountAsync();
        }

        // ----------------------------------------
        // Get Pohotovost Technik IDs
        // ----------------------------------------
        public async Task<Technik> GetPohotovostTechnikIdsAsync(string Id)
        {
            var technikIds = await _context.PohotovostiS
                .Select(p => p.Technik.ID)
                .Distinct()
                .ToListAsync();

            return _mapper.Map<Technik>(technikIds);
        }

        // ----------------------------------------
        // Get Pohotovost Technik ID list
        // ----------------------------------------
        public async Task<List<string>> GetPohotovostTechnikIdListAsync()
        {
            var technikIds = await _context.PohotovostiS
            .Select(p => p.Technik.ID)
            .Distinct()
            .ToListAsync();

            return _mapper.Map<List<string>>(technikIds);
        }

        // ----------------------------------------
        // Get Technik-Lokalita map
        // ----------------------------------------
        public async Task<Dictionary<string, string>> GetTechnikLokalitaMapAsync(List<string> technikIds)
        {
            return await _context.DieslovaniS
                .Include(o => o.Odstavka).ThenInclude(l => l.Lokality)
                .Where(d => technikIds.Contains(d.IdTechnik))
                .GroupBy(d => d.IdTechnik)
                .ToDictionaryAsync(
                    group => group.Key,
                    group => group
                        .OrderBy(o => o.Odstavka.Od)
                        .Where(o => o.Vstup == null)
                        .Select(d => d.Odstavka.Lokality.Nazev)
                        .FirstOrDefault() ?? string.Empty
                );
        }
        public async Task<string> GetTechnikVPohotovostiAsnyc(int firmaid)
        {
            var technik = await _context.PohotovostiS
                .Include(o => o.Technik)
                .ThenInclude(o=> o.User)
                .Include(o => o.Technik)
                .ThenInclude(o => o.Firma)
                .Where(o => o.Technik.Firma.ID == firmaid && o.Technik.Taken==false)
                .FirstOrDefaultAsync();
                if (technik == null)
                {
                    return "0";
                }
            return technik.Technik.ID;
        }
        // ----------------------------------------
        // Get Pohotovost table data
        // ----------------------------------------
        public async Task<List<object>> GetPohotovostTableDataAsync(int start, int length, Dictionary<string, string> technikLokalitaMap)
        {
            return await _context.PohotovostiS
                .Include(o => o.Technik).ThenInclude(o => o.User)
                .Include(o => o.Technik).ThenInclude(o => o.Firma)
                .OrderBy(o => o.Zacatek)
                .Skip(start)
                .Take(length)
                .Select(l => (object)new
                {
                    JmenoTechnika = l.Technik.User.Jmeno,
                    PrijmeniTechnika = l.Technik.User.Prijmeni,
                    TelTechnika = l.Technik.User.PhoneNumber,
                    FirmaTechnika = l.Technik.Firma.Nazev,
                    ZacetekPohotovosti = l.Zacatek,
                    KonecPohotovosti = l.Konec,
                    TechnikStatus = l.Technik.Taken,
                    Lokalita = technikLokalitaMap.ContainsKey(l.Technik.ID)
                        ? technikLokalitaMap[l.Technik.ID]
                        : "Nemá přiřazenou lokalitu"
                })
                .ToListAsync();
        }
    }
}