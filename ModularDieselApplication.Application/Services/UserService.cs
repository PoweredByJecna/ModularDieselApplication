using System.Threading.Tasks;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<object> DetailUserJsonAsync(string userId)
        {
            // 1) Načtení "hlavních" dat uživatele z DB
            var userDetail = await _userRepository.GetByIdAsync(userId);
            if (userDetail == null)
            {
                return new { error = "Uživatel nenalezen" };
            }
            // 2) Role
            var role = await _userRepository.GetUserPrimaryRoleAsync(userId);

            // 3) Pohotovost
            var pohotovost = await _userRepository.GetUserPohotovostAsync(userId);

            // 4) Technik + Firma
            var technik = await _userRepository.GetUserTechnikAsync(userId);
            var firma = technik?.Firma; // Pokud TableTechnik.Firma existuje

            // 5) Region
            var region = firma != null
                ? await _userRepository.GetUserRegionForFirmaAsync(firma.ID)
                : null;

            // 6) Poskládání JSON-like objektu
            return new 
            {
                uzivatelskeJmeno = userDetail.UserName,
                stav = (pohotovost != null),
                nadrizeny = "Neznámý", // Původní kód takto vracel
                firma = firma?.Nazev ?? "Neznámá",
                region = region?.Nazev ?? "Neznámý",
                jmeno = pohotovost?.Technik?.User.Jmeno,
                prijmeni = pohotovost?.Technik?.User.Prijmeni,
                tel = userDetail.PhoneNumber,
                role = role
            };
        }

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)
        {
            var userRole = await _userRepository.GetUserPrimaryRoleAsync(userId);
            if (userRole == null) return false;
            return userRole.Equals(roleName, System.StringComparison.OrdinalIgnoreCase);
        }

  
    
    }
}
