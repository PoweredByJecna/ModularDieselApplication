using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Objects;

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
        public async Task<HandleResult> VstupAsync(string idDieslovani)
        {
           
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null)
                {
                    dis.Vstup = DateTime.Now;
                    dis.Technik.Taken = true;

                    await _dieslovaniRepository.UpdateAsync(dis);
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " vstoupil na lokalitu.");

                    return HandleResult.OK("Byl zadán vstup na lokalitu.");
                }
                else
                {
                    return HandleResult.Error("Záznam dieslování nebyl nalezen.");
                
                }
            }
            catch (Exception ex)
            {
                return HandleResult.Error($"Chyba při zadávání vstupu: {ex.Message}");
            }
        }

        // ----------------------------------------
        // Record exit from a location.
        // ----------------------------------------
        public async Task<HandleResult> OdchodAsync(string idDieslovani)
        {
           
            try
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

                    dis.Odchod = DateTime.Now;
                    await _dieslovaniRepository.UpdateAsync(dis);
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " zadal odchod z lokality.");

                    return HandleResult.OK("Byl zadán odchod z lokality.");
                }
                else
                {
                    return HandleResult.Error("Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return HandleResult.Error($"Chyba při zadávání odchodu: {ex.Message}");
            }
        }

        // ----------------------------------------
        // Assign a technician to a location.
        // ----------------------------------------
        public async Task<HandleResult> TakeAsync(string idDieslovani, User currentUser)
        {
            try
            {
                if (string.IsNullOrEmpty(currentUser.Id))
                {
                    return HandleResult.Error("ID aktuálního uživatele není platné.");
                }
                var technik = await _technikService.GetTechnikByUserIdAsync(currentUser.Id);
                var dieslovaniTaken = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dieslovaniTaken == null)
                {
                    return HandleResult.Error("Záznam dieslování nebyl nalezen.");
                }
                if (technik == null)
                {
                    return HandleResult.Error("Technik nebyl nalezen.");
                }
                var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);
                if (!pohotovostTechnik)
                {
                    return HandleResult.Error("Nejste zapsán v pohotovosti.");
                }
                dieslovaniTaken.Technik = technik;
                dieslovaniTaken.Technik.Taken = true;
                await _technikService.UpdateTechnikAsync(dieslovaniTaken.Technik);
                await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovaniTaken.ID, $"Technik {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni} si převzal lokalitu.");

               return HandleResult.OK($"Lokalitu si převzal: {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni}");
            }
            catch (Exception ex)
            {
                return HandleResult.Error($"Chyba při převzetí: {ex.Message}");
            }
        }

        // ----------------------------------------
        // Delete a dieslovani record.
        // ----------------------------------------
        public async Task<HandleResult> DeleteDieslovaniAsync(string id)
        {
            try
            {
                var dieslovani = await _dieslovaniRepository.GetByIdAsync(id);
                if (dieslovani == null)
                {
                    return HandleResult.Error("Dieslovani nebyla nalezena.");
                }

                bool deleted = await _dieslovaniRepository.DeleteAsync(id);

                if (!deleted)
                {
                    return HandleResult.Error("Dieslovani se nepodařilo smazat.");
                }

                return HandleResult.OK("Dieslovani byla úspěšně smazána.");
            }
            catch (Exception ex)
            {
                return HandleResult.Error($"Chyba při mazání dieslovani: {ex.Message}");
            }
        }

        // ----------------------------------------
        // Change the time for a dieslovani record.
        // ----------------------------------------
        public async Task<HandleResult> ChangeTimeAsync(string idDieslovani, DateTime time, string type)
        {

            try
            {
                var dieslovani = await _dieslovaniRepository.GetByIdAsync(idDieslovani);
                if (dieslovani == null)
                {
                    return HandleResult.Error("Dieslovani nebyla nalezena.");
                }

                if (type == "vstup")
                {
                    if (time.Date != dieslovani.Odstavka.Od.Date)
                    {
                        return HandleResult.Error("Nelze zadat vstup mimo den odstávky.");
                    }

                    dieslovani.Vstup = time;
                }
                else if (type == "odchod")
                {
                    if (dieslovani.Vstup == DateTime.MinValue)
                    {
                        return HandleResult.Error("Nejprve musíte zadat vstup.");
                    }
                    dieslovani.Odchod = time;
                }
                else
                {
                    return HandleResult.Error("Neznámý typ času.");
                }

                await _dieslovaniRepository.UpdateAsync(dieslovani);

                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Byl změnen čas {type}, na {time}.");
                return HandleResult.OK($"Čas {type} byl úspěšně změněn na {time}.");
            }

            catch (Exception ex)
            {
                return HandleResult.Error($"Chyba při změně času: {ex.Message}");
            }
        }
    }
}