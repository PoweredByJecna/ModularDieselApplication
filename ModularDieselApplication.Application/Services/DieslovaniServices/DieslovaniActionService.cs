using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Rules;
using static ModularDieselApplication.Application.Services.OdstavkyService;

namespace ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniActionService
{
    public class DieslovaniActionService
    {
        private readonly IDieslovaniRepository _dieslovaniRepository; 
        private readonly ITechnikService _technikService;

        public DieslovaniActionService(IDieslovaniRepository dieslovaniRepository, ITechnikService technikService)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _technikService = technikService;
        }
        /* ----------------------------------------
            VstupAsync
        ---------------------------------------- */
        public async Task<(bool Success, string Message)> VstupAsync(int idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null)
                {
                    dis.Vstup = DateTime.Now;
                    dis.Technik.Taken = true;
    
                    await _dieslovaniRepository.UpdateAsync(dis);
                    return (true, "Byl zadán vstup na lokalitu.");
                }
                else
                {
                    return (false, "Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při zadávání vstupu " + ex.Message);
            }
        }
        /*----------------------------------------
            OdchodAsync
        ----------------------------------------*/
        public async Task<(bool Success, string Message)> OdchodAsync(int idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null)
                {              
                    var anotherDiesel = await _dieslovaniRepository.AnotherDieselRequest(dis.Technik.ID);


                    if (anotherDiesel == false)
                    {
                        dis.Technik.Taken = false;
                        await _technikService.UpdateTechnikAsync(dis.Technik);
                    }

                    dis.Odchod = DateTime.Now;
                    await _dieslovaniRepository.UpdateAsync(dis);

                    return (true, "Byl zadán odchod z lokality.");
                }
                else
                {
                    return (false, "Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při zadávání odchodu " + ex.Message);
            }
        }
        /* ----------------------------------------
           TemporaryLeaveAsync
        ---------------------------------------- */
        public async Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                dis.Technik.Taken = !dis.Technik.Taken;

                await _dieslovaniRepository.UpdateAsync(dis);
                return (true, $"Změněn stav technika (Taken = {!dis.Technik.Taken}).");
            }
            catch (Exception ex)
            {
                return (false, "Chyba při dočasném uvolnění: " + ex.Message);
            }
        }
        /* ----------------------------------------
           TakeAsync
        ---------------------------------------- */
        public async Task<(bool Success, string Message, string? TempMessage)> TakeAsync(int idDieslovani, User currentUser)
        {
            try
            {
                var technik = await _technikService.GetTechnikByIdAsync(currentUser.Id);

                var dieslovaniTaken = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dieslovaniTaken == null)
                {
                    return (false, "Záznam dieslování nebyl nalezen.", null);
                }

                var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);

                if (!pohotovostTechnik)
                {
                    return (false, "Nejste zapsán v pohotovosti.", null);
                }

                if (technik.Taken)
                {
                    return (false, "Již máte převzaté jiné dieslování.", null);
                }

                dieslovaniTaken.Technik = technik;
                technik.Taken = true;
                await _technikService.UpdateTechnikAsync(technik);
                await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);

                return (true, $"Lokalitu si převzal: {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Jmeno}", 
                             "Dieslování bylo úspěšně zadáno.");
            }
            catch (Exception ex)
            {
                return (false, $"Chyba při převzetí: {ex.Message}", null);
            }
        }
        /*----------------------------------------
           DeleteDieslovaniJsonAsync
        ---------------------------------------- */
        public async Task<(bool Success, string Message)> DeleteDieslovaniAsync(int id)
        {
            try
            {
                var dieslovani = await _dieslovaniRepository.GetByIdAsync(id);
                if (dieslovani == null)
                {
                    return (false, "Dieslovani nebylo nalezeno.");
                }

                bool deleted = await _dieslovaniRepository.DeleteAsync(id);
                if (!deleted)
                {
                    return (false, "Dieslovani se nepodařilo smazat.");
                }

                return (true, "Dieslovani byla úspěšně smazána.");
            }
            catch (Exception ex)
            {
                return (false, $"Chyba při mazání dieslovani: {ex.Message}");
            }
        }

    }
}