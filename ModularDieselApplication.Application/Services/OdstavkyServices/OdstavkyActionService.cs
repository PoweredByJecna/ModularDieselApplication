
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
        private readonly DieslovaniRules _dieslovaniRules;

        public OdstavkyActionService(IOdstavkyRepository odstavkaRepository, IDieslovaniService dieslovaniService, ITechnikService technikService, DieslovaniRules dieslovaniRules)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniService = dieslovaniService;
            _technikService = technikService;
            _dieslovaniRules = dieslovaniRules;

        }

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

                if(dieslovani == null)
                {
                    await _odstavkaRepository.DeleteAsync(idodstavky);
                    result.Success = true;
                    return result;
                }

                var technikID = dieslovani.Technik.ID;
                await _odstavkaRepository.DeleteAsync(idodstavky);

                if (dieslovani != null)
                {
                    
                    var antoherDa= await _dieslovaniService.AnotherDieselRequestAsync(technikID);
                    if(!antoherDa)
                    {
                        var Technik = await _technikService.GetTechnikByIdAsync(technikID);
                        Technik.Taken = false;
                        await _technikService.UpdateTechnikAsync(Technik);
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
                    odstavka.Od = time;
                    await _dieslovaniRules.IsDieselRequired(odstavka.Lokality.Klasifikace, odstavka.Od, odstavka.Do, odstavka.Lokality.Baterie, odstavka, result);
                    if (!result.Success)
                    {
                        
                    }
                    
                }
                else if (type == "konec")
                {
                    odstavka.Do = time;
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