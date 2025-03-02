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

                await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Dieslování není potřeba z důvodu: {result.Duvod}");

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

                    await _logService.ZapisDoLogu(DateTime.Now.Date, "Dieslovani", dieslovani.ID, "Nepodařilo se přiřadit technika.");

                    result.Message = "Nepodařilo se přiřadit technika.";

                    return result;
                }
                else
                {
                    dieslovani.Technik = technikSearch;
                    await _dieslovaniRepository.UpdateDieslovaniAsync(dieslovani);
                    result.Message = "Vytvořeno nové dieslování.";
                    result.Success = true;
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

                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Firma která bude zajišťovat dieslování: {firmaVRegionu.Nazev}");

                var technikId = await _pohotovostiService.GetTechnikActivTechnikByIdFirmaAsync(firmaVRegionu.ID);

                var technikSearch = await _technikService.GetTechnikByIdAsync(technikId);

                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, "Hledání technika který bude zajišťovat dieslování.");

                if (technikId == "0")
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, "Nebyl nalezen aktivní technik, který by byl volný");
                    bool nejakyTechnikMaPohotovost = await _pohotovostiService.PohovostiVRegionuAsync(firmaVRegionu.ID);
                    
                    if (nejakyTechnikMaPohotovost)
                    {
                        await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, "Nějaký technik má pohotovost.");
                        technikSearch = await CheckTechnikReplacementAsync(dieslovani.Odstavka);
                        return technikSearch;
                    }
                    else
                    {
                        await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, "Žádný technik v regionu nemá pohotovost.");
                        //await _emailService.SendEmailAsync("Dieslování", "Žádný technik v regionu nemá pohotovost.");
                        technikSearch = await _technikService.GetTechnikByIdAsync("606794494");
                        return technikSearch;
                    }
                    
                }
                else
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Technik: {technikSearch.User.Jmeno} {technikSearch.User.Prijmeni} byl přiřazen k dieslování.");
                    return technikSearch;
                }
            }
            else
            {
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, "Chyba při přiřzení technika");
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
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", newOdstavka.ID, "Hledání technika, který má dieslovaní s nižšší prioritou.");
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
            if (newOdstavka.Lokality.Region.Firma == null)
            {
                return null;
            }
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
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", newDieslovani.ID, $"Nové dieslování č.{newDieslovani.ID} bylo vytvořeno.");

            return newDieslovani;
        }



    }
}