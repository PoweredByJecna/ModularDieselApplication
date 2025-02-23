using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Application.Interfaces.Repositories;
using AutoMapper;
using ModularDieselApplication.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ModularDieselApplication.Infrastructure.Repositories
{
    public class LokalityRepository : ILokalityRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        public LokalityRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        public async Task<List<object>> GetAllLokalityAsync()
        {
            var entities = await _context.LokalityS
                .Include(l => l.Region)
                .Select(l => new 
                {
                    nazev = l.Nazev,
                    klasifikace = l.Klasifikace,
                    adresa = l.Adresa,
                    Baterie = l.Baterie,
                    Zasuvka = l.Zasuvka,
                    region = l.Region.Nazev,
                    zdroj = l.Zdroj != null ? l.Zdroj.Nazev : "N/A"
              
                })
                .ToListAsync();
            return _mapper.Map<List<object>>(entities);
        }
    }
}