using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;

namespace ModularDieselApplication.Application.Services
{
    public class OdstavkyActionService 
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly IDieslovaniService _dieslovaniService;
        private readonly ITechnikService _technikService;
        private readonly ILogService _logService;

        public OdstavkyActionService(IOdstavkyRepository odstavkaRepository, IDieslovaniService dieslovaniService, ITechnikService technikService, ILogService logService)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniService = dieslovaniService;
            _technikService = technikService;
            _logService = logService;
        }

        // ----------------------------------------
        // Delete an odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> DeleteOdstavkaAsync(int idodstavky)
        {
            var result = new HandleResult();
            try
            {
                var odstavka = await _odstavkaRepository.GetByIdAsync(idodstavky);
                if (odstavka == null)
                {
                    result.Success = false;
                    result.Message = "Záznam nebyl nalezen.";
                    return result;
                }
                
                var dieslovani = await _dieslovaniService.GetDieslovaniByOdstavkaId(idodstavky);

                if (dieslovani == null)
                {
                    await _odstavkaRepository.DeleteAsync(idodstavky);
                    result.Success = true;
                    return result;
                }

                var technikID = dieslovani.Technik.ID;
                await _odstavkaRepository.DeleteAsync(idodstavky);

                if (dieslovani != null)
                {
                    var anotherDa = await _dieslovaniService.AnotherDieselRequestAsync(technikID);
                    if (!anotherDa)
                    {
                        var technik = await _technikService.GetTechnikByIdAsync(technikID);
                        technik.Taken = false;
                        await _technikService.UpdateTechnikAsync(technik);
                    }
                }
                result.Success = true;
                result.Message = "Záznam byl úspěšně smazán.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Chyba při mazání záznamu: " + ex.Message;
                return result;
            }
        }

        // ----------------------------------------
        // Change the time for an odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> ChangeTimeOdstavkyAsync(int idodstavky, DateTime time, string type)
        {
            var result = new HandleResult();
            try
            {
                var odstavka = await _odstavkaRepository.GetByIdAsync(idodstavky);
                if (odstavka == null)
                {
                    result.Success = false;
                    result.Message = "Záznam nebyl nalezen.";
                    return result;
                }
                if (type == "zacatek")
                {
                    if (odstavka.Od.Date < DateTime.Today.Date)
                    {
                        result.Success = false;
                        result.Message = "Nelze měnit čas již ukončené odstávky.";
                        return result;
                    }
                    odstavka.Od = time;
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", odstavka.ID, $"Byl změněn čas začátku odstávky na: {odstavka.Od}");

                    await _dieslovaniService.HandleOdstavkyDieslovani(odstavka, result);
                }
                else if (type == "konec")
                {
                    if (odstavka.Od.Date < DateTime.Today.Date)
                    {
                        result.Success = false;
                        result.Message = "Nelze měnit čas již ukončené odstávky.";
                        return result;
                    }
                    odstavka.Do = time;
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", odstavka.ID, $"Byl změněn čas konce odstávky na: {odstavka.Do}");
                    await _dieslovaniService.HandleOdstavkyDieslovani(odstavka, result);
                }
                await _odstavkaRepository.UpdateAsync(odstavka);
                result.Success = true;
                result.Message = "Čas byl úspěšně změněn.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Chyba při změně času: " + ex.Message;
                return result;
            }
        }
    }
}