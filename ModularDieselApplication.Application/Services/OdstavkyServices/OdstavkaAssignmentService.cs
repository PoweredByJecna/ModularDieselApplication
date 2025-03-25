using System.Security.Cryptography;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Rules;

namespace ModularDieselApplication.Application.Services
{
    public class OdstavkaAssignmentService
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly DieslovaniAssignmentService _dieslovaniAssignmentService;
        private readonly IDieslovaniService _dieslovaniService;
        private readonly ILogService _logService;
        private readonly ITechnikService _technikService;

        public OdstavkaAssignmentService(IOdstavkyRepository odstavkaRepository, DieslovaniAssignmentService dieslovaniAssignmentService, IDieslovaniService dieslovaniService, ILogService logService, ITechnikService technikService)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniAssignmentService = dieslovaniAssignmentService;
            _dieslovaniService = dieslovaniService;
            _logService = logService;
            _technikService = technikService;
        }

        // ----------------------------------------
        // Create a new odstávka.
        // ----------------------------------------
        public async Task<HandleResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option)
        {
            var result = new HandleResult();
            try
            {
                var lokalitaSearch = await _odstavkaRepository.GetByNameAsync(lokalita);

                if (lokalitaSearch == null)
                {
                    result.Success = false;
                    result.Message = "Lokalita nenalezena.";
                    return result;
                }

                result = await OdstavkyRules.OdstavkyCheck(od, @do, result, await ExistingOdstavka(lokalitaSearch.ID, od));
                if (!result.Success)
                    return result;

                string distrib = DetermineDistributor(lokalitaSearch.Region.Nazev);

                var newOdstavka = await CreateNewOdstavka(lokalitaSearch, distrib, od, @do, popis);

                if (option == "now")
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Dieslování objednáno bez kontroli");

                    var technik = await _technikService.GetTechnikByIdAsync("606794494");

                    if (technik == null)
                    {
                        result.Success = false;
                        result.Message = "Technik se nanašel.";
                        return result;
                    }

                    var dieslovani = await _dieslovaniAssignmentService.CreateNewDieslovaniAsync(newOdstavka, technik);

                    await _dieslovaniAssignmentService.AssignTechnikAsync(dieslovani, newOdstavka);

                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka a dieslování byly úspěšně vytvořeny.";
                    return result;
                }
                else
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Kontrola, zda je dieslovaní potřeba.");

                    result = await _dieslovaniService.HandleOdstavkyDieslovani(newOdstavka, result);

                    return result;
                }
            }
            catch
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba {result.Message}";
                return result;
            }
        }

        // ----------------------------------------
        // Check if an existing odstávka overlaps.
        // ----------------------------------------
        private async Task<bool> ExistingOdstavka(int lokalitaSearchId, DateTime od)
        {
            var existingOdstavka = await _odstavkaRepository.AnotherOdsatvkaAsync(lokalitaSearchId, od);

            return existingOdstavka == null;
        }

        // ----------------------------------------
        // Determine the distributor based on the region name.
        // ----------------------------------------
        public string DetermineDistributor(string NazevRegionu)
        {
            return NazevRegionu switch
            {
                "Severní Čechy" or "Západní Čechy" or "Severní Morava" => "ČEZ",
                "Jižní Morava" or "Jižní Čechy" => "EGD",
                "Praha + Střední Čechy" => "PRE",
                _ => ""
            };
        }

        // ----------------------------------------
        // Test the creation of an odstávka.
        // ----------------------------------------
        public async Task<HandleResult> TestOdstavkaAsync()
        {
            var result = new HandleResult();
            var number = await _odstavkaRepository.GetLokalitaCountAsync();

            var IdNumber = RandomNumberGenerator.GetInt32(1, number);

            var lokalitaSearch = await _odstavkaRepository.GetLokalityByIdAsync(IdNumber);

            var hours = RandomNumberGenerator.GetInt32(1, 100);

            string distrib = DetermineDistributor(lokalitaSearch.Region.Nazev);

            var od = DateTime.Today.AddHours(hours + 2);

            var do_ = DateTime.Today.AddHours(hours + 8);

            string popis = $"Odstávka od {distrib}, od: {od}, do: {do_}";

            string option = "Default";

            result = await CreateOdstavkaAsync(lokalitaSearch.Nazev, od, do_, popis, option);

            return result;
        }

        // ----------------------------------------
        // Create a new odstávka record.
        // ----------------------------------------
        private async Task<Odstavka> CreateNewOdstavka(Lokalita lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            var newOdstavka = new Odstavka
            {
                Distributor = distrib,
                Od = od,
                Do = do_,
                Popis = popis,
                Lokality = lokalitaSearch
            };

            try
            {
                await _odstavkaRepository.AddAsync(newOdstavka);

                await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Nová odstávka č.{newOdstavka.ID} bylo vytvořeno.");
                await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Vytvřáření odstávky s parametry: Lokalita: {newOdstavka.Lokality.Nazev}, Klasifikace: {newOdstavka.Lokality.Klasifikace}, Od: {newOdstavka.Od}, Do: {newOdstavka.Do}");
                await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Baterie: {newOdstavka.Lokality.Baterie} min");
            }
            catch (Exception)
            {
                throw new Exception("Chyba při ukládání do databáze");
            }

            return newOdstavka;
        }
    }
}