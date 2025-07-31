using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;


namespace ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniActionService
{
    public class DieslovaniActionService
    {
        private readonly IDieslovaniRepository _dieslovaniRepository; 
        private readonly ITechnikService _technikService;
        private readonly ILogService _logService;

        public DieslovaniActionService(IDieslovaniRepository dieslovaniRepository, ITechnikService technikService, ILogService logService)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _technikService = technikService;
            _logService = logService;
        }

        // ----------------------------------------
        // Record entry to a location.
        // ----------------------------------------
        public async Task VstupAsync(string idDieslovani)
        {
            var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

            if (dis != null)
            {
                dis.Nastav(DieslovaniFieldEnum.Vstup,DateTime.Now);
                dis.Technik.Taken = true;
                await _dieslovaniRepository.UpdateAsync(dis);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " vstoupil na lokalitu.");
            }
            else
            {
                throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            }
        }
        // ----------------------------------------
        // Record exit from a location.
        // ----------------------------------------
        public async Task OdchodAsync(string idDieslovani)
        {
            var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);
            if (dis != null)
            {
                var anotherDiesel = await _dieslovaniRepository.AnotherDieselRequest(dis.Technik.ID);

                if (!anotherDiesel)
                {
                    dis.Technik.Taken = false;
                    await _technikService.UpdateTechnikAsync(dis.Technik);
                }

                dis.Nastav(DieslovaniFieldEnum.Odchod, DateTime.Now);
                await _dieslovaniRepository.UpdateAsync(dis);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " zadal odchod z lokality.");
            }
            else
            {
                throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            }
        }
        // ----------------------------------------
        // Assign a technician to a location.
        // ----------------------------------------
        public async Task TakeAsync(string idDieslovani, User currentUser)
        {
          
                var technik = await _technikService.GetTechnik(GetTechnikEnum.ByUserId, currentUser.Id);
                var dieslovaniTaken = await _dieslovaniRepository.GetByIdAsync(idDieslovani);
                var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);
                if (!pohotovostTechnik)
                {
                    throw new InvalidOperationException("Technik není v pohotovosti.");
                }        
                dieslovaniTaken.Technik = technik;
                dieslovaniTaken.Technik.Taken = true;
                await _technikService.UpdateTechnikAsync(dieslovaniTaken.Technik);
                await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovaniTaken.ID, $"Technik {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni} si převzal lokalitu.");  
        }

        // ----------------------------------------
        // Delete a dieslovani record.
        // ----------------------------------------
        public async Task DeleteDieslovaniAsync(string id)
        {
            var dieslovani = await _dieslovaniRepository.GetByIdAsync(id);
            if (dieslovani == null)
            {
                throw new InvalidOperationException($"Dieslovani with id {id} not found.");
            }
            bool deleted = await _dieslovaniRepository.DeleteAsync(id);
            if (!deleted)
            {
                throw new InvalidOperationException($"Failed to delete Dieslovani with id {id}.");
            }
        }

        // ----------------------------------------
        // Change the time for a dieslovani record.
        // ----------------------------------------
        public async Task ChangeTimeAsync(string idDieslovani, DateTime time, ActionFilter type)
        {
            var dieslovani = await _dieslovaniRepository.GetByIdAsync(idDieslovani) ?? throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            switch (type)
            {
                case ActionFilter.Vstup:
                    if (time.Date != dieslovani.Odstavka.Od.Date)
                    {
                        throw new InvalidOperationException("Vstup musí být v den odstávky.");
                    }
                    dieslovani.Nastav(DieslovaniFieldEnum.Vstup, time);
                    break;
                case ActionFilter.Odchod:
                    if (dieslovani.Vstup == DateTime.MinValue)
                    {
                        throw new InvalidOperationException("Nejprve musíte zadat vstup.");
                    }
                    dieslovani.Nastav(DieslovaniFieldEnum.Odchod, time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            await _dieslovaniRepository.UpdateAsync(dieslovani);
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Byl změnen čas na {time}.");
        }
            
        
    }
}