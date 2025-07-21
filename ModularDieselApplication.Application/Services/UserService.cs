using System.Threading.Tasks;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<HandleResult> ChangePasswordAsync(string userId, string newPassword)
        {
            return await _userRepository.ChangePasswordAsync(userId, newPassword);
        }

        public async Task<HandleResult> AddUserAsync(string username, string password, string email, string role, string Jmeno, string Prijmeni)
        {
            User user = new ()
            {
                UserName = username,
                Password = password,
                Email = email,
                Jmeno = Jmeno,
                Prijmeni = Prijmeni
            };
            return await _userRepository.AddAsync(user);
        }


        // ----------------------------------------
        // Get dieslovani records associated with a user as JSON.
        // ----------------------------------------
        public async Task<List<object>> VazbyJsonAsync(string userId)
        {
            var userDieslovaniList = await _userRepository.GetDieslovaniByUserId(userId);
            return userDieslovaniList;
        }

        // ----------------------------------------
        // Get detailed information about a user as JSON.
        // ----------------------------------------
        public async Task<object> DetailUserJsonAsync(string userId)
        {
            var userDetail = await _userRepository.GetByIdAsync(userId);
            if (userDetail == null)
            {
                return new { error = "Uživatel nenalezen" };
            }

            var role = await _userRepository.GetUserPrimaryRoleAsync(userId);
            var pohotovost = await _userRepository.GetUserPohotovostAsync(userId);
            var technik = await _userRepository.GetUserTechnikAsync(userId);
            var firma = technik?.Firma;
            var region = firma != null
                ? await _userRepository.GetUserRegionForFirmaAsync(firma.ID)
                : null;

            return new
            {
                uzivatelskeJmeno = userDetail.UserName,
                stav = technik?.Taken,
                nadrizeny = "Neznámý", 
                firma = firma?.Nazev ?? "Neznámá",
                region = region?.Nazev ?? "Neznámý",
                jmeno = pohotovost?.Technik?.User.Jmeno,
                prijmeni = pohotovost?.Technik?.User.Prijmeni,
                tel = userDetail.PhoneNumber,
                Role = role,
                PohotovostZacatek = pohotovost?.Zacatek,
                PohotovostKonec = pohotovost?.Konec,
            };
        }

        // ----------------------------------------
        // Check if a user belongs to a specific role.
        // ----------------------------------------
        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var userRole = await _userRepository.GetUserPrimaryRoleAsync(userId);
            if (userRole == null) return false;
            return userRole.Equals(roleName,StringComparison.OrdinalIgnoreCase);
        }
    }
}
