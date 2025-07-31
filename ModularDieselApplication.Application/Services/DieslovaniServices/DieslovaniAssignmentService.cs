using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Rules;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;
using System.Reflection.Metadata;

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
        public async Task<HandleResult<Dieslovani>> HandleOdstavkyDieslovani(Odstavka newOdstavka)
        {
            var technik = await _technikService.GetTechnikByIdAsync(FiktivniTechnik.Id);

            if (newOdstavka == null)
            {
                return HandleResult<Dieslovani>.Error("Odstavka is null.");
            }

            var result = await _dieslovaniRules.IsDieselRequired(newOdstavka.Lokality.Klasifikace, newOdstavka.Od, newOdstavka.Do, newOdstavka.Lokality.Baterie, newOdstavka);
            {
                switch (result)
                {
                    case IsDieselRequiredEnum.Zasuvka:
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Dieslování není potřeba z důvodu že na lokalitě není zásuvka.");
                        return HandleResult<Dieslovani>.Error($"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nDieslování není potřeba z důvodu, protože na lokalitě není zásuvka.");

                    case IsDieselRequiredEnum.Agregat:
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Dieslování není potřeba z důvodu že na lokalitě je diesel agregát.");
                        return HandleResult<Dieslovani>.Error($"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nDieslování není potřeba z důvodu, protože na lokalitě je diesel agregát.");

                    case IsDieselRequiredEnum.Baterie:
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Dieslování je částečně potřeba: na lokalitě jsou baterie, které vydrží po dobu odstávky.");
                        return HandleResult<Dieslovani>.Error($"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nDieslování je částečně potřeba: na lokalitě jsou baterie, které vydrží po dobu odstávky.");

                    default:
                        var dieslovani = await CreateNewDieslovaniAsync(newOdstavka, technik);
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, $"Bylo vytvořeno nové dieslování č.{dieslovani.ID}.");
                        return HandleResult<Dieslovani>.OK(dieslovani, $"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nBylo vytvořeno nové dieslování č.{dieslovani.ID}.\nTechnik: {technikSearch.User.Jmeno} {technikSearch.User.Jmeno}.");
                }
            }
           
        }

        // ----------------------------------------
        // Assign a technician to dieslovani.
        // ----------------------------------------
        public async Task<Technik> AssignTechnikAsync(Dieslovani dieslovani, Odstavka newOdstavka)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(dieslovani.Odstavka.Lokality.Region.ID);

            if (firmaVRegionu != null)
            {
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", dieslovani.ID, $"Firma která bude zajišťovat dieslování: {firmaVRegionu.Nazev}");
                var technik = await _pohotovostiService.GetTechnikActivTechnikByIdFirmaAsync(firmaVRegionu.ID, newOdstavka.Od, newOdstavka.Do); 
                if(technik.ID == FiktivniTechnik.Id)
                {
                    technik = await CheckTechnikReplacementAsync(dieslovani, dieslovani.Odstavka, firmaVRegionu.ID)
                
                
                
                    await SaveTechnikAndDieslovani(dieslovani, technikSearch);
                    return technikSearch;
                
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
        private async Task<Firma?> GetFirmaVRegionuAsync(string regionId)
        {
            return await _regionyService.GetFirmaVRegionuAsync(regionId);
        }

        // ----------------------------------------
        // Check for a technician replacement.
        // ----------------------------------------
        private async Task CheckTechnikReplacementAsync(Dieslovani dieslovani, Odstavka newOdstavka, string idFirma)
        {
            await GetHigherPriorityAsync(dieslovani, newOdstavka, idFirma);
        }

        // ----------------------------------------
        // Get a technician with higher priority.
        // ----------------------------------------
        private async Task GetHigherPriorityAsync(Dieslovani newdieslovani, Odstavka newOdstavka, string idFirma)
        {
            var dieslovani = await _dieslovaniRepository.GetDieslovaniWithTechnikAsync(idFirma);
            if (dieslovani.Odstavka.Do < newOdstavka.Od.AddHours(3) || newOdstavka.Do < dieslovani.Odstavka.Od.AddHours(4))
            {
                await SaveTechnikAndDieslovani(newdieslovani, dieslovani.Technik);
            }
            int staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
            int novaVaha = newOdstavka.Lokality.Klasifikace.ZiskejVahu();
            bool maVyssiPrioritu = novaVaha > staraVaha;
            bool casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) > DateTime.Now.Date;

            if (maVyssiPrioritu && casovyLimit)
            {
                var novyTechnik = await _technikService.GetTechnik(GetTechnikEnum.fitkivniTechnik);
                await SaveTechnikAndDieslovani(newdieslovani, dieslovani.Technik);
                await SaveTechnikAndDieslovani(dieslovani, novyTechnik);
            }
            else
            {
                var novyTechnik = await _technikService.GetTechnikByIdAsync(FiktivniTechnik.Id);
                await SaveTechnikAndDieslovani(newdieslovani, novyTechnik);
            }
        }

        // ----------------------------------------
        // Create a new dieslovani record.
        // ----------------------------------------
        public async Task<Dieslovani> CreateNewDieslovaniAsync(Odstavka newOdstavka, Technik technik)
        {
            Dieslovani newDieslovani = new()
            { 
                Odstavka = newOdstavka,
                Technik = technik
            }; 
            await _dieslovaniRepository.AddAsync(newDieslovani);
            technik.Taken = true;
            var technikSearch = await AssignTechnikAsync(newDieslovani, newOdstavka);

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
        public async Task CallDieslovaniAsync(string idodstavky)
        {
            try
            {
                var existingDieslovani = await AnotherDieselRequest(idodstavky);
                if (existingDieslovani)
                {
                    return HandleResult.Error( $"Dieslování pro tuto odstávku je již vytvořeno");
                }
                var odstavka = await _dieslovaniRepository.GetByOdstavkaByIdAsync(idodstavky);

                if (odstavka.Do.Date < DateTime.Now.Date)
                {
                    return HandleResult.Error("Odstávka již skončila.");
                }
                var technik = await _technikService.GetTechnikByIdAsync(FiktivniTechnik.Id);

                if (technik == null)
                {
                    return HandleResult.Error("Technik nebyl nalezen.");
                }

                var dieslovani = await CreateNewDieslovaniAsync(odstavka, technik);
                await AssignTechnikAsync(dieslovani, odstavka);
                return HandleResult.OK();
            }
            catch (Exception ex)
            {
                return HandleResult.Error("Chyba při vytváření dieslování: " + ex.Message);
            }
        }

        // ----------------------------------------
        // Check if another dieslovani request exists.
        // ----------------------------------------
        private async Task<bool> AnotherDieselRequest(string odstavka)
        {
            var dieslovani = await _dieslovaniRepository.GetDAbyOdstavkaAsync(odstavka);
            return dieslovani != null;
        }
    }
}