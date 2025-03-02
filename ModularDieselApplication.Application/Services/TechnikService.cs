using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Interfaces.Repositories;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Services
{
    public class TechnikService : ITechnikService
    {
        private readonly ITechniciRepository _techniciRepository;

        public TechnikService(ITechniciRepository techniciRepository)
        {
            _techniciRepository = techniciRepository;
        }

        public async Task<Technik?> GetTechnikByIdAsync(string idtechnika)
        {
            var technik =  await _techniciRepository.GetByIdAsync(idtechnika);
            if (technik != null)
            {
                await _techniciRepository.UpdateAsync(technik);
            }
            return technik;

        }
        public async Task<Technik> GetTechnikByIdFrimy(int idFirmy)
        {
            return await _techniciRepository.GetByFirmaIdAsync(idFirmy);
        }

        public async Task<Technik?> GetTechnikByUserIdAsync(string idUser)
        {
            return await _techniciRepository.GetByUserIdAsync(idUser);
        }

        public async Task<bool> IsTechnikOnDutyAsync(string idTechnika)
        {
            return await _techniciRepository.IsTechnikOnDutyAsync(idTechnika);
        }

        public async Task UpdateTechnikAsync(Technik technik)
        {
            await _techniciRepository.UpdateAsync(technik);
        }

        // Other method implementations...
    }
}