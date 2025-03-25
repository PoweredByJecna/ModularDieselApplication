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
        private readonly UserManager<TableUser> _userManager;

        public UserRepository(ApplicationDbContext context, IMapper mapper, UserManager<TableUser> userManager, SignInManager<TableUser> signInManager)
        {
            _context = context;
            _mapper = mapper;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ----------------------------------------
        // Log in a user.
        // ----------------------------------------
        public async Task<SignInResult> LoginAsync(string username, string password, bool rememberMe)
        {
            return await _signInManager.PasswordSignInAsync(username, password, rememberMe, false);
        }

        // ----------------------------------------
        // Get a user by their username.
        // ----------------------------------------
        public async Task<User?> GetUserByUsernameAsync(string username)
        {
            var tableUser = await _userManager.FindByNameAsync(username);
            return tableUser is null ? null : _mapper.Map<User>(tableUser);
        }

        // ----------------------------------------
        // Log out the current user.
        // ----------------------------------------
        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }

        // ----------------------------------------
        // Get a user by their ID.
        // ----------------------------------------
        public async Task<User?> GetByIdAsync(string userId)
        {
            var userEntity = await _context.Users
                .FirstOrDefaultAsync(u => u.Id == userId);
            return _mapper.Map<User?>(userEntity);
        }

        // ----------------------------------------
        // Get the primary role of a user.
        // ----------------------------------------
        public async Task<string?> GetUserPrimaryRoleAsync(string userId)
        {
            var userRoleEntity = await _context.UserRoles
                .FirstOrDefaultAsync(ur => ur.UserId == userId);
            var roleName = await _context.Roles
                .FirstOrDefaultAsync(r => r.Id == userRoleEntity.RoleId);
            return roleName?.Name;
        }

        // ----------------------------------------
        // Get the Pohotovost record associated with a user.
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
        // Get the Technik record associated with a user.
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
        // Get the Region associated with a user's Firma.
        // ----------------------------------------
        public async Task<Region?> GetUserRegionForFirmaAsync(int firmaId)
        {
            var regionEntity = await _context.RegionS
                .Include(r => r.Firma)
                .Where(r => r.FirmaID == firmaId)
                .FirstOrDefaultAsync();

            return _mapper.Map<Region?>(regionEntity);
        }

        // ----------------------------------------
        // Get Dieslovani records associated with a user.
        // ----------------------------------------
        public async Task<List<object>> GetDieslovaniByUserId(string userId)
        {
            var userDieslovani = await _context.DieslovaniS
                .Include(d => d.Technik)
                    .ThenInclude(t => t.User)
                .Where(d => d.Technik.User.Id == userId)
                .Select(l => new
                {
                    id = l.ID,
                    distributor = l.Odstavka.Distributor,
                    lokalitaNazev = l.Odstavka.Lokality.Nazev,
                    klasifikace = l.Odstavka.Lokality.Klasifikace,
                    adresa = l.Odstavka.Lokality.Adresa,
                    technikFirma = l.Technik.Firma.Nazev,
                    jmenoTechnika = l.Technik.User.Jmeno,
                    prijmeniTechnika = l.Technik.User.Prijmeni,
                    zadanVstup = l.Vstup,
                    zadanOdchod = l.Odchod,
                    Idtechnika = l.Technik.ID,
                    NazevRegionu = l.Odstavka.Lokality.Region.Nazev,
                    OdstavkaZacatek = l.Odstavka.Od,
                    OdstavkaKonec = l.Odstavka.Do,
                    Popis = l.Odstavka.Popis,
                    VydrzBaterie = l.Odstavka.Lokality.Baterie,
                    Zasuvka = l.Odstavka.Lokality.Zasuvka,
                    User = l.Technik.User.Id
                })
                .ToListAsync();

            return _mapper.Map<List<object>>(userDieslovani);
        }
    }
}