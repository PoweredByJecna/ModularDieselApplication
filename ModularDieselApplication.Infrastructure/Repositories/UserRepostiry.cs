
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using ModularDieselApplication.Application.Interfaces.Repositories;
using AutoMapper;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Persistence.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly SignInManager<TableUser> _signInManager;
        private readonly UserManager<TableUser>_userManager;
        public UserRepository (ApplicationDbContext context, IMapper mapper, UserManager<TableUser> userManager, SignInManager<TableUser> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        // ----------------------------------------
        // LogIn User
        // ----------------------------------------
        public async Task<SignInResult> LoginAsync(string username, string password, bool rememberMe)
        {
            // Tady už signInManager nebude null
            return await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);
        }
        // ----------------------------------------
        // GetUserByUsernameAsync
        // ----------------------------------------
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var tableUser = await _userManager.FindByNameAsync(username);
            return tableUser is null ? null : _mapper.Map<User>(tableUser);
        }
        // ----------------------------------------
        // Logout User
        // ----------------------------------------
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
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
            var  rolename = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == userRoleEntity.RoleId);    
            return rolename?.Name;
        }

        // ----------------------------------------
        // Get User's Pohotovost
        // ----------------------------------------
        public async Task<Pohotovosti?> GetUserPohotovostAsync(string userId)
        {
            var pohotovost = await _context.PohotovostiS
                .Include(o => o.Technik)
                    .ThenInclude(t => t.User)
                .Where(o => o.Technik.IdUser == userId)
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