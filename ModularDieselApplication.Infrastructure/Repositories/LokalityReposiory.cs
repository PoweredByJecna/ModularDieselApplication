using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Application.Interfaces.Repositories;
using AutoMapper;
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
        public async Task<List<Lokalita>> GetAllLokalityAsync()
        {
            var entities = await _context.LokalityS
                .Include(l => l.Region)
                .ToListAsync();
            return _mapper.Map<List<Lokalita>>(entities);
        }
    }
}