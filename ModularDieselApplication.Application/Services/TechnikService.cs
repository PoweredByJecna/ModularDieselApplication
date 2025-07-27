using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Interfaces.Repositories;
using ModularDieselApplication.Domain.Enum;
using System.Threading.Tasks;

namespace ModularDieselApplication.Application.Services
{
    public class TechnikService : ITechnikService
    {
        private readonly ITechniciRepository _techniciRepository;
        private readonly ILogService _logService;

        public TechnikService(ITechniciRepository techniciRepository, ILogService logService)
        {
            _techniciRepository = techniciRepository;
            _logService = logService;
        }


        // ----------------------------------------
        // Get a technician by their ID.
        // ----------------------------------------
        public async Task<Technik> GetTechnikByIdAsync(string idtechnika)
        {
            var technik = await _techniciRepository.GetByIdAsync(idtechnika);
            if (technik != null)
            {
                await _techniciRepository.UpdateAsync(technik);
                return technik;
            }
            else
            {
                throw new KeyNotFoundException($"Technician with ID {idtechnika} not found.");
            }
        }

        // ----------------------------------------
        // Get a technician by their firm's ID.
        // ----------------------------------------
        public async Task<Technik> GetTechnikByIdFrimy(string idFirmy)
        {
            return await _techniciRepository.GetByFirmaIdAsync(idFirmy);
        }

        // ----------------------------------------
        // Get a technician by their user ID.
        // ----------------------------------------
        public async Task<Technik> GetTechnikByUserIdAsync(string idUser)
        {
            return await _techniciRepository.GetByUserIdAsync(idUser);
        }

        // ----------------------------------------
        // Check if a technician is currently on duty.
        // ----------------------------------------
        public async Task<bool> IsTechnikOnDutyAsync(string idTechnika)
        {
            return await _techniciRepository.IsTechnikOnDutyAsync(idTechnika);
        }

        // ----------------------------------------
        // Update a technician's details.
        // ----------------------------------------
        public async Task UpdateTechnikAsync(Technik technik)
        {
            await _techniciRepository.UpdateAsync(technik);
        }
        
        public async Task<Technik> GetTechnik(GetTechnikEnum getTecnikEnum, string? idTechnika = null, string? idUser = null, string? idFirmy = null)
        {
            switch (getTecnikEnum)
            {
                case GetTechnikEnum.fitkivniTechnik:
                    string fiktivniTechnikID = "a3d5e0c7-2b1a-4d6a-a914-3c13a15f8497";
                    return await _techniciRepository.GetByIdAsync(fiktivniTechnikID);
                case GetTechnikEnum.ByUserId:
                    if (string.IsNullOrEmpty(idUser))
                    {
                        throw new ArgumentException("idUser cannot be null or empty when using GetTechnikEnum.ByUserId.");
                    }
                    return await _techniciRepository.GetByUserIdAsync(idUser);
                case GetTechnikEnum.ByFirmaId:
                    if (string.IsNullOrEmpty(idFirmy))
                    {
                        throw new ArgumentException("idFirmy cannot be null or empty when using GetTechnikEnum.ByFirmaId.");
                    }
                    return await _techniciRepository.GetByFirmaIdAsync(idFirmy);
                case GetTechnikEnum.ById:
                    if (string.IsNullOrEmpty(idTechnika))
                    {
                        throw new ArgumentException("idTechnika cannot be null or empty when using GetTechnikEnum.ById.");
                    }
                    return await _techniciRepository.GetByIdAsync(idTechnika);
                default:
                    throw new ArgumentException("Invalid GetTechnikEnum value.");
            }
        }
    }
}