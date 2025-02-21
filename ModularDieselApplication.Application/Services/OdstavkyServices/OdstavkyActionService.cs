
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Objects;

namespace ModularDieselApplication.Application.Services
{
    public class OdstavkyActionService 
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly IDieslovaniService _dieslovaniService;
        private readonly ITechnikService _technikService;

        public OdstavkyActionService(IOdstavkyRepository odstavkaRepository, IDieslovaniService dieslovaniService, ITechnikService technikService)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniService = dieslovaniService;
            _technikService = technikService;

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

                // Případně zrušení dieslování a uvolnění technika
                var dieslovani = await _dieslovaniService.GetDieslovaniByOdstavkaId(idodstavky);
                if (dieslovani != null)
                {
                    var technik = await _technikService.GetTechnikByIdAsync(dieslovani.Technik.ID);
                    if (technik != null)
                    {
                        technik.Taken = false;
                        await _technikService.UpdateTechnikAsync(technik);
                    }
                }

                await _odstavkaRepository.DeleteAsync(idodstavky);

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

    }
    
}