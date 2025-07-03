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
            var result = new HandleResult();
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null)
                {
                    dis.Vstup = DateTime.Now;
                    dis.Technik.Taken = true;

                    await _dieslovaniRepository.UpdateAsync(dis);
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " vstoupil na lokalitu.");

                    result.Success = true;
                    result.Message = "Byl zadán vstup na lokalitu.";
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Záznam dieslování nebyl nalezen.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Chyba při zadávání vstupu " + ex.Message;
                return result;
            }
        }

        // ----------------------------------------
        // Record exit from a location.
        // ----------------------------------------
        public async Task<HandleResult> OdchodAsync(string idDieslovani)
        {
            var result = new HandleResult();
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
                    result.Success = true;
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " zadal odchod z lokality.");

                    result.Message = "Byl zadán odchod z lokality.";
                    return result;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Záznam dieslování nebyl nalezen.";
                    return result;
                }
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Chyba při zadávání odchodu " + ex.Message;
                return result;
            }
        }

        // ----------------------------------------
        // Toggle temporary leave status.
        // ----------------------------------------
        public async Task<(bool Success, string Message)> TemporaryLeaveAsync(string idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null && dis.Technik != null)
                {
                    dis.Technik.Taken = !dis.Technik.Taken;

                    await _dieslovaniRepository.UpdateAsync(dis);
                    return (true, $"Změněn stav technika (Taken = {!dis.Technik.Taken}).");
                }
                else
                {
                    return (false, "Záznam dieslování nebo technik nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při dočasném uvolnění: " + ex.Message);
            }
        }

        // ----------------------------------------
        // Assign a technician to a location.
        // ----------------------------------------
        public async Task<HandleResult> TakeAsync(string idDieslovani, User currentUser)
        {
            var result = new HandleResult();
            try
            {
                if (string.IsNullOrEmpty(currentUser.Id))
                {
                    result.Success = false;
                    result.Message = "ID aktuálního uživatele není platné.";
                    return result;
                }

                var technik = await _technikService.GetTechnikByUserIdAsync(currentUser.Id);
                var dieslovaniTaken = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dieslovaniTaken == null)
                {
                    result.Success = false;
                    result.Message = "Záznam dieslování nebyl nalezen.";
                    return result;
                }

                if (technik == null)
                {
                    result.Success = false;
                    result.Message = "Technik nebyl nalezen.";
                    return result;
                }

                var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);

                if (!pohotovostTechnik)
                {
                    result.Success = false;
                    result.Message = "Nejste zapsán v pohotovosti.";
                    return result;
                }

                dieslovaniTaken.Technik = technik;
                dieslovaniTaken.Technik.Taken = true;
                await _technikService.UpdateTechnikAsync(dieslovaniTaken.Technik);
                await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovaniTaken.ID, $"Technik {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni} si převzal lokalitu.");

                result.Success = true;
                result.Message = $"Lokalitu si převzal: {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Chyba při převzetí: {ex.Message}";
                return result;
            }
        }

        // ----------------------------------------
        // Delete a dieslovani record.
        // ----------------------------------------
        public async Task<HandleResult> DeleteDieslovaniAsync(string id)
        {
            var result = new HandleResult();
            try
            {
                var dieslovani = await _dieslovaniRepository.GetByIdAsync(id);
                if (dieslovani == null)
                {
                    result.Success = false;
                    result.Message = "Dieslovani nebyla nalezena.";
                    return result;
                }

                bool deleted = await _dieslovaniRepository.DeleteAsync(id);

                if (!deleted)
                {
                    result.Success = false;
                    result.Message = "Dieslovani se nepodařilo smazat.";
                    return result;
                }

                result.Success = true;
                result.Message = "Dieslovani byla úspěšně smazána.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Chyba při mazání dieslovani: {ex.Message}";
                return result;
            }
        }

        // ----------------------------------------
        // Change the time for a dieslovani record.
        // ----------------------------------------
        public async Task<HandleResult> ChangeTimeAsync(string idDieslovani, DateTime time, string type)
        {
            var result = new HandleResult();
            try
            {
                var dieslovani = await _dieslovaniRepository.GetByIdAsync(idDieslovani);
                if (dieslovani == null)
                {
                    result.Success = false;
                    result.Message = "Dieslovani nebyla nalezena.";
                    return result;
                }

                if (type == "vstup")
                {
                    if (time.Date != dieslovani.Odstavka.Od.Date)
                    {
                        result.Success = false;
                        result.Message = "Nelze zadat vstup mimo den odstávky.";
                        return result;
                    }

                    dieslovani.Vstup = time;
                }
                else if (type == "odchod")
                {
                    if (dieslovani.Vstup == DateTime.MinValue)
                    {
                        result.Success = false;
                        result.Message = "Nejprve musíte zadat vstup.";
                        return result;
                    }
                    dieslovani.Odchod = time;
                }
                else
                {
                    result.Success = false;
                    result.Message = "Neznámý typ času.";
                    return result;
                }

                await _dieslovaniRepository.UpdateAsync(dieslovani);
                result.Success = true;
                result.Message = $"Čas {type} byl změněn na {time}.";
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Byl změnen čas {type}, na {time}.");
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Chyba při změně času: {ex.Message}";
                return result;
            }
        }
    }
}