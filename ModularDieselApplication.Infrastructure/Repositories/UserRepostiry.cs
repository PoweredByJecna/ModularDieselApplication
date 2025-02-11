using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using AutoMapper;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<TableUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public UserRepository(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ----------------------------------------
        // Get User by ID
        // ----------------------------------------
        public async Task<User?> GetByIdAsync(string userId)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            return _mapper.Map<User?>(userEntity);
        }

        // ----------------------------------------
        // Get User's primary role
        // ----------------------------------------
        public async Task<string?> GetUserPrimaryRoleAsync(string userId)
        {
            var userRoleEntity = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);
            return userRoleEntity?.RoleId;
        }

        // ----------------------------------------
        // Get User's Pohotovost
        // ----------------------------------------
        public async Task<Pohotovosti?> GetUserPohotovostAsync(string userId)
        {
            var pohotovost = await _context.PohotovostiS
                .Include(o => o.Technik)
                    .ThenInclude(t => t.User)
                .Where(o => o.User.Id == userId)
                .FirstOrDefaultAsync();

            return _mapper.Map<Pohotovosti?>(pohotovost);
        }

        // ----------------------------------------
        // Get User's Technik
        // ----------------------------------------
        public async Task<Technik?> GetUserTechnikAsync(string userId)
        {
            var technik = await _context.TechnikS
                .Include(t => t.Firma)
                .Where(t => t.IdUser == userId)
                .FirstOrDefaultAsync();

            return _mapper.Map<Technik?>(technik);
        }

        // ----------------------------------------
        // Get User's Region for Firma
        // ----------------------------------------
        public async Task<Region?> GetUserRegionForFirmaAsync(int firmaId)
        {
            var regionEntity = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.FirmaID == firmaId)
                .FirstOrDefaultAsync();

            return _mapper.Map<Region?>(regionEntity);
        }
    }
}