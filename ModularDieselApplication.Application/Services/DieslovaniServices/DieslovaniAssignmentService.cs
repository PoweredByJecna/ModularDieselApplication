using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Rules;
using ModularDieselApplication.Domain.Objects;


namespace ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService
{
    public class DieslovaniAssignmentService
    {
        private readonly IDieslovaniRepository _dieslovaniRepository; 
        private readonly ITechnikService _technikService;
        private readonly IPohotovostiService _pohotovostiService;
        private readonly IEmailService _emailService;
        private readonly DieslovaniRules _dieslovaniRules;
        private readonly IRegionyService _regionyService;
        private readonly ILogService _logService;

        public DieslovaniAssignmentService(IDieslovaniRepository dieslovaniRepository, ITechnikService technikService, IPohotovostiService pohotovostiService, IEmailService emailService, DieslovaniRules dieslovaniRules, IRegionyService regionyService, ILogService logService)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _technikService = technikService;
            _pohotovostiService = pohotovostiService;
            _emailService = emailService;
            _dieslovaniRules = dieslovaniRules;
            _regionyService = regionyService;
            _logService = logService;
        }
        public async Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result)
        {
            if (newOdstavka == null)
            {
                result.Success = false;
                result.Message = "Odstavka is null.";
                return result;
            }

            await DieslovaniRules.IsDieselRequired(newOdstavka.Lokality.Klasifikace, newOdstavka.Od, newOdstavka.Do, newOdstavka.Lokality.Baterie, newOdstavka, result);
           
            if(!result.Success)
            {
                result.Success = false;

                result.Message = $"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nDieslování není potřeba z důvodu: {result.Duvod}";

                result.Color="Orange";

                return result;
            }

            var technik = await _technikService.GetTechnikByIdAsync("606794494");

            if (technik == null)
            {
                result.Success = false;

                result.Message = "Technik se nanašel.";

                return result;
            }

            else 
            {
                var dieslovani = await CreateNewDieslovaniAsync(newOdstavka, technik);

                var technikSearch = await AssignTechnikAsync(dieslovani);

                if (technikSearch == null)
                {
                    result.Success = false;

                    result.Message = "Nepodařilo se přiřadit technika.";

                    return result;
                }
                else
                {
                    result.Success = true;

                    var EmailResult = "DA-ok";

                    await _emailService.SendDieslovaniEmailAsync(dieslovani, EmailResult);

                    result.Message = $"Dieslování č. {dieslovani.ID} bylo úspěšně vytvořeno.";

                    return result;
                }
            }
        }
        /* ----------------------------------------
           AssignTechnikAsync
           ---------------------------------------- */
        public async Task<Technik?> AssignTechnikAsync(Dieslovani dieslovani)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(dieslovani.Odstavka.Lokality.Region.ID);
            
            if (firmaVRegionu != null)
            {
                var technikSearch = await _pohotovostiService.GetTechnikActivTechnikByIdFirmaAsync(firmaVRegionu.ID);

                if (technikSearch == null)
                {
                    
                    bool nejakyTechnikMaPohotovost = await _pohotovostiService.PohovostiVRegionuAsync(firmaVRegionu.ID);
                    
                    if (nejakyTechnikMaPohotovost)
                    {
                        technikSearch = await CheckTechnikReplacementAsync(dieslovani.Odstavka);
                        if (technikSearch != null)
                        {
                            return technikSearch;
                        }
                        return technikSearch;
                    }
                    return technikSearch;
                }
                else
                {
                    return technikSearch;
                }
            }
            else
            {
                await _logService.ZapisDoLogu(DateTime.Now.Date, "Dieslovani", dieslovani.ID, "Chyba při přiřzení technika");

                return null;
            }
        }
        /* ----------------------------------------
           GetFirmaVRegionuAsync
        ---------------------------------------- */
        private async Task<Firma?> GetFirmaVRegionuAsync(int regionId)
        {
            return await _regionyService.GetFirmaVRegionuAsync(regionId);
        }
        /* ----------------------------------------
           CheckTechnikReplacementAsync
           ---------------------------------------- */
        private async Task<Technik?> CheckTechnikReplacementAsync(Odstavka newOdstavka)
        {
            var technik = await GetHigherPriorityAsync(newOdstavka);
            if (technik == null)
            {
                return null;
            }
            else
            {
                return technik;
            }
        }
        /* ----------------------------------------
           GetHigherPriorityAsync
           ---------------------------------------- */
        private async Task<Technik?> GetHigherPriorityAsync(Odstavka newOdstavka)
        {
            var dieslovani = await _dieslovaniRepository.GetDieslovaniWithTechnikAsync(newOdstavka.Lokality.Region.Firma.ID);

            if (dieslovani == null)
            {
                return null;
            }

            if (dieslovani.Odstavka.Do < newOdstavka.Od.AddHours(3) || newOdstavka.Do < dieslovani.Odstavka.Od.AddHours(4))
            {
                return dieslovani.Technik;
            }

            int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
            int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();
            bool maVyssiPrioritu = novaVaha > staraVaha;
            bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) < DateTime.Now;
            bool daPodminka = dieslovani.Odstavka.Lokality.DA == false;

            if (maVyssiPrioritu && casovyLimit && daPodminka)
            {
                var novyTechnik = await _technikService.GetTechnikByIdAsync("606794494");
                if (novyTechnik != null)
                {
                    await CreateNewDieslovaniAsync(newOdstavka, dieslovani.Technik);
                    dieslovani.Technik = novyTechnik;
                    await _dieslovaniRepository.UpdateDieslovaniAsync(dieslovani);
                }
                return novyTechnik;
            }
            else
            {
                var novyTechnik = await _technikService.GetTechnikByIdAsync("606794494");
                return novyTechnik;
            }
            
        }
        public async Task<Dieslovani> CreateNewDieslovaniAsync(Odstavka newOdstavka, Technik technik)
        {
            var newDieslovani = new Dieslovani
            {
                Vstup = DateTime.MinValue,
                Odchod = DateTime.MinValue,
                Odstavka = newOdstavka,
                Technik = technik
            };
            await _dieslovaniRepository.AddAsync(newDieslovani);
            technik.Taken = true;
            await _technikService.UpdateTechnikAsync(technik);

            return newDieslovani;
        }



    }
}