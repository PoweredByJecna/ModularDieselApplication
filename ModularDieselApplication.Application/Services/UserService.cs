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
        public async Task<List<object>> VazbyJsonAsync(string userId)
        {
            var userDieslovaniList = await _userRepository.GetUserDieslovaniAsync(userId);
            return userDieslovaniList;
        } 

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
                nadrizeny = "Neznámý", // Původní kód takto vracel
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

        public async Task<bool> IsUserInRoleAsync(string userId, string roleName)   
        {
            var userRole = await _userRepository.GetUserPrimaryRoleAsync(userId);
            if (userRole == null) return false;
            return userRole.Equals(roleName, System.StringComparison.OrdinalIgnoreCase);
        }
    
    }
}
