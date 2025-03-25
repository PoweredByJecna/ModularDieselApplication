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
        private readonly IRegionyService _regionyService;
        private readonly ILogService _logService;
        private readonly DieslovaniRules _dieslovaniRules;

        public DieslovaniAssignmentService(IDieslovaniRepository dieslovaniRepository,
         ITechnikService technikService, IPohotovostiService pohotovostiService,
           IRegionyService regionyService, ILogService logService,
           DieslovaniRules dieslovaniRules)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _technikService = technikService;
            _pohotovostiService = pohotovostiService;
            _regionyService = regionyService;
            _logService = logService;
            _dieslovaniRules = dieslovaniRules;
        }

        // ----------------------------------------
        // Handle dieslovani for an odstávka.
        // ----------------------------------------
        public async Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result)
        {
            var technik = await _technikService.GetTechnikByIdAsync("606794494");

            if (newOdstavka == null)
            {
                result.Success = false;
                result.Message = "Odstavka is null.";
                return result;
            }

            await _dieslovaniRules.IsDieselRequired(newOdstavka.Lokality.Klasifikace, newOdstavka.Od, newOdstavka.Do, newOdstavka.Lokality.Baterie, newOdstavka, result);
           
            if (!result.Success)
            {
                result.Success = false;
                await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Dieslování není potřeba z důvodu: {result.Duvod}");
                result.Message = $"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nDieslování není potřeba z důvodu: {result.Duvod}";
                result.Color = "Orange";
                return result;
            }

            if (technik == null)
            {
                result.Success = false;
                result.Message = "Technik se nanašel.";
                return result;
            }
            else 
            {
                var dieslovani = await CreateNewDieslovaniAsync(newOdstavka, technik);
                var technikSearch = await AssignTechnikAsync(dieslovani, newOdstavka);

                if (technikSearch == null)
                {
                    result.Success = false;
                    await _logService.ZapisDoLogu(DateTime.Now.Date, "Dieslovaní", dieslovani.ID, "Nepodařilo se přiřadit technika.");
                    result.Message = "Nepodařilo se přiřadit technika.";
                    return result;
                }
                else
                {
                    await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Bylo vytvořeno nové dieslování č.{dieslovani.ID}.");
                    result.Message = "Vytvořeno nové dieslování.";
                    result.Success = true;
                    return result;
                }
            }
        }

        // ----------------------------------------
        // Assign a technician to dieslovani.
        // ----------------------------------------
        public async Task<Technik?> AssignTechnikAsync(Dieslovani dieslovani, Odstavka newOdstavka)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(dieslovani.Odstavka.Lokality.Region.ID);

            if (firmaVRegionu != null)
            {
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", dieslovani.ID, $"Firma která bude zajišťovat dieslování: {firmaVRegionu.Nazev}");

                var technikId = await _pohotovostiService.GetTechnikActivTechnikByIdFirmaAsync(firmaVRegionu.ID, newOdstavka.Od, newOdstavka.Do);   
                var technikSearch = await _technikService.GetTechnikByIdAsync(technikId);

                if (technikId == "0")
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", dieslovani.ID, "Nebyl nalezen aktivní technik, který by byl volný");
                    bool nejakyTechnikMaPohotovost = await _pohotovostiService.PohovostiVRegionuAsync(firmaVRegionu.ID, newOdstavka.Od, newOdstavka.Do);
                    
                    if (nejakyTechnikMaPohotovost)
                    {
                        technikSearch = await CheckTechnikReplacementAsync(dieslovani, dieslovani.Odstavka, firmaVRegionu.ID);
                        return technikSearch;
                    }
                    else
                    {
                        technikSearch = await _technikService.GetTechnikByIdAsync("606794494");
                        return technikSearch;
                    }
                }
                else
                {
                    await SaveTechnikAndDieslovani(dieslovani, technikSearch);
                    return technikSearch;
                }
            }
            else
            {
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, "Chyba při přiřazení technika");
                return null;
            }
        }

        // ----------------------------------------
        // Get a company in the region.
        // ----------------------------------------
        private async Task<Firma?> GetFirmaVRegionuAsync(int regionId)
        {
            return await _regionyService.GetFirmaVRegionuAsync(regionId);
        }

        // ----------------------------------------
        // Check for a technician replacement.
        // ----------------------------------------
        private async Task<Technik?> CheckTechnikReplacementAsync(Dieslovani dieslovani, Odstavka newOdstavka, int idFirma)
        {
            var technik = await GetHigherPriorityAsync(dieslovani, newOdstavka, idFirma);
            return technik;
        }

        // ----------------------------------------
        // Get a technician with higher priority.
        // ----------------------------------------
        private async Task<Technik?> GetHigherPriorityAsync(Dieslovani newdieslovani, Odstavka newOdstavka, int idFirma)
        {
            var dieslovani = await _dieslovaniRepository.GetDieslovaniWithTechnikAsync(idFirma);

            if (dieslovani == null)
            {
                return null;
            }

            if (dieslovani.Odstavka.Do < newOdstavka.Od.AddHours(3) || newOdstavka.Do < dieslovani.Odstavka.Od.AddHours(4))
            {
                await SaveTechnikAndDieslovani(newdieslovani, dieslovani.Technik);
                return dieslovani.Technik;
            }

            int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
            int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();
            bool maVyssiPrioritu = novaVaha > staraVaha;
            bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) > DateTime.Now.Date;

            if (maVyssiPrioritu && casovyLimit)
            {
                var novyTechnik = await _technikService.GetTechnikByIdAsync("606794494");
                await SaveTechnikAndDieslovani(newdieslovani, dieslovani.Technik);
                await SaveTechnikAndDieslovani(dieslovani, novyTechnik);
                return newdieslovani.Technik;
            }
            else
            {
                var novyTechnik = await _technikService.GetTechnikByIdAsync("606794494");
                await SaveTechnikAndDieslovani(newdieslovani, novyTechnik);
                return newdieslovani.Technik;
            }
        }

        // ----------------------------------------
        // Create a new dieslovani record.
        // ----------------------------------------
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
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", newDieslovani.ID, $"Nové dieslování č.{newDieslovani.ID} bylo vytvořeno.");

            return newDieslovani;
        }

        // ----------------------------------------
        // Save technician and dieslovani details.
        // ----------------------------------------
        private async Task SaveTechnikAndDieslovani(Dieslovani newdieslovani, Technik novyTechnik)
        {
            newdieslovani.Technik = novyTechnik;
            newdieslovani.Technik.Taken = true;
            await _technikService.UpdateTechnikAsync(newdieslovani.Technik);
            await _dieslovaniRepository.UpdateDieslovaniAsync(newdieslovani);
        }

        // ----------------------------------------
        // Call dieslovani for an odstávka.
        // ----------------------------------------
        public async Task<HandleResult> CallDieslovaniAsync(int idodstavky)
        {
            var result = new HandleResult();
            try
            {
                var existingDieslovani = await AnotherDieselRequest(idodstavky);
                if (existingDieslovani)
                {
                    result.Success = false;
                    result.Message = "Dieslování pro tuto odstávku je již vytvořeno.";
                    return result;
                }
                var odstavka = await _dieslovaniRepository.GetByOdstavkaByIdAsync(idodstavky);

                if (odstavka.Do.Date < DateTime.Now.Date)
                {
                    result.Success = false;
                    result.Message = "Odstávka již skončila.";
                    return result;
                }
                var technik = await _technikService.GetTechnikByIdAsync("606794494");

                if (technik == null)
                {
                    result.Success = false;
                    result.Message = "Technik nebyl nalezen.";
                    return result;
                }

                var dieslovani = await CreateNewDieslovaniAsync(odstavka, technik);
                await AssignTechnikAsync(dieslovani, odstavka);
                result.Success = true;  
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = "Chyba při vytváření dieslování: " + ex.Message;
                return result;
            }
        }

        // ----------------------------------------
        // Check if another dieslovani request exists.
        // ----------------------------------------
        private async Task<bool> AnotherDieselRequest(int odstavka)
        {
            var dieslovani = await _dieslovaniRepository.GetDAbyOdstavkaAsync(odstavka);
            return dieslovani != null;
        }
    }
}