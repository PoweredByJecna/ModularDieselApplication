using System.Security.Cryptography;
using System.Threading.Tasks;
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
        private readonly OdstavkyRules _odstavkyRules;

        public OdstavkaAssignmentService(IOdstavkyRepository odstavkaRepository, DieslovaniAssignmentService dieslovaniAssignmentService, IDieslovaniService dieslovaniService, ILogService logService, OdstavkyRules odstavkyRules)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniAssignmentService = dieslovaniAssignmentService;
            _dieslovaniService = dieslovaniService;
            _logService = logService;
            _odstavkyRules = odstavkyRules;
        }

        public async Task<HandleOdstavkyDieslovaniResult> CreateOdstavkaAsync(string lokalita, DateTime od, DateTime @do, string popis, string option)
        {
            var result = new HandleOdstavkyDieslovaniResult();
            try
            {
                // Najdeme danou lokalitu
                var lokalitaSearch = await _odstavkaRepository.GetByNameAsync(lokalita);

                if (lokalitaSearch == null)
                {
                    result.Success = false;
                    result.Message = "Lokalita nenalezena.";
                    return result;
                }

                
                // Kontrola termínů a existence odstávky
                result = _odstavkyRules.OdstavkyCheck(lokalitaSearch, od, @do, result, await ExistingOdstavka(lokalitaSearch.ID, od));
                if (!result.Success)
                    return result;

                // Který distributor
                string distrib = DetermineDistributor(lokalitaSearch.Region.Nazev);

                // Vytvoříme novou odstávku
                var newOdstavka = await CreateNewOdstavka(lokalitaSearch, distrib, od, @do, popis);

                if(option=="now")
                {
                    var technik = await _dieslovaniAssignmentService.AssignTechnikAsync(newOdstavka);
                    if (technik == null)
                    {
                        result.Success = false;
                        result.Message = "Něco se pokazilo";
                        return result;
                    }
                    var dieslovani = await _dieslovaniAssignmentService.CreateNewDieslovaniAsync(newOdstavka, technik);
                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka a dieslování byly úspěšně vytvořeny.";
                    return result;
                }
                else
                {
                    result = await _dieslovaniService.HandleOdstavkyDieslovani(newOdstavka, result);
                    return result;
                }
              
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba: {ex.Message}, {result.Message}, {result.Duvod}";
                return result;
            }
        }
        private async Task<bool> ExistingOdstavka(int lokalitaSearchId, DateTime od)
        {
 
            var existingOdstavka =  await _odstavkaRepository.AnotherOdsatvkaAsync(lokalitaSearchId,od);

            if (existingOdstavka == null)
            {
                return  true;
            }
            else
            {
                return false;
            }
        }
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
        public async Task<HandleOdstavkyDieslovaniResult> TestOdstavkaAsync()
        {
            var result = new HandleOdstavkyDieslovaniResult();

            try
            {
                var number = await _odstavkaRepository.GetLokalitaCountAsync();
                if (number == 0)
                {
                    result.Success = false;
                    result.Message = "Žádné lokality v DB.";
                    return result;
                }

                var IdNumber = RandomNumberGenerator.GetInt32(1, number);
                var lokalitaSearch = await _odstavkaRepository.GetLokalityByIdAsync(IdNumber);

                if (lokalitaSearch == null)
                {
                    result.Success = false;
                    result.Message = "Lokalita nenalezena.";
                    return result;
                }

                // Generujeme náhodné časy
                var hours = RandomNumberGenerator.GetInt32(1, 100);
                string distrib = DetermineDistributor(lokalitaSearch.Region.Nazev);
                var od = DateTime.Today.AddHours(hours + 2);
                var do_ = DateTime.Today.AddHours(hours + 8);
                string popis = $"Odstávka od {distrib}, od: {od}, do: {do_}";

                // Kontrola
                result = _odstavkyRules.OdstavkyCheck(lokalitaSearch, od, do_, result, await ExistingOdstavka(lokalitaSearch.ID, od));
                if (!result.Success)
                    return result;

                // Vytvoříme novou odstávku
                var newOdstavka = await CreateNewOdstavka(lokalitaSearch, distrib, od, do_, popis);

                try
                {
                    await _odstavkaRepository.AddAsync(newOdstavka);
                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka byla úspěšně vytvořena.";
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.Message = "Chyba při ukládání do databáze";
                    return result;
                }
                if (newOdstavka != null && newOdstavka.Lokality != null && newOdstavka.Lokality.Region != null)
                {
                    var id = newOdstavka.ID;
                    await _logService.ZapisDoLogu(DateTime.Now.Date, "odstávka", id, $"Vytvřáření odstávky s parametry: Lokalita: {newOdstavka.Lokality.Nazev}, Klasifikace: {newOdstavka.Lokality?.Klasifikace}, Od: {newOdstavka?.Od}, Do: {newOdstavka?.Do}");
                    await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", id, $"Baterie: {newOdstavka?.Lokality?.Baterie} min");

                    result = await _dieslovaniService.HandleOdstavkyDieslovani(newOdstavka, result);
                    if (!result.Success)
                    {
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "odstávka", newOdstavka.ID, result.Message);
                    }
                    else
                    {
                        await _logService.ZapisDoLogu(DateTime.Now.Date, "Odstávka", newOdstavka.ID, result.Message);
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba: {ex.Message}";
                return result;
            }
        }

        private async Task<Odstavka> CreateNewOdstavka(Lokalita lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            var result = new HandleOdstavkyDieslovaniResult();
            
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
                    result.Odstavka = newOdstavka;
                    result.Message = "Odstávka byla úspěšně vytvořena.";
                }
                catch (Exception)
                {
                    result.Success = false;
                    result.Message = "Chyba při ukládání do databáze";
                }
            return newOdstavka;
        }

    }
}