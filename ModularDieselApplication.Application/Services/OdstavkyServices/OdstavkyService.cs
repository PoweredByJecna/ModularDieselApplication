using System.Diagnostics;
using System.Security.Cryptography;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;


namespace ModularDieselApplication.Application.Services
{
    public class OdstavkyService : IOdstavkyService
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly IDieslovaniService _dieslovaniService;
        private readonly ILogService _logService;
        private readonly ITechnikService _technikService;
        private readonly DieslovaniAssignmentService _dieslovaniAssignmentService;

        public OdstavkyService(IOdstavkyRepository odstavkaRepository, IDieslovaniService dieslovaniService, ILogService logService, ITechnikService technikService, DieslovaniAssignmentService dieslovaniAssignmentService)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniService = dieslovaniService;
            _logService = logService;
            _technikService = technikService;
            _dieslovaniAssignmentService = dieslovaniAssignmentService;

        }
        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
            var lokalities = await _odstavkaRepository.GetAllAsync();
            
            return [.. lokalities
                .Where(l => l.Lokality.Nazev.Contains(query))
                .Select(l => l.Lokality.Nazev)
                .Take(10)];
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
                result = OdstavkyCheck(lokalitaSearch, od, @do, result);
                if (!result.Success)
                    return result;

                // Který distributor
                string distrib = DetermineDistributor(lokalitaSearch.Region.Nazev);

                // Vytvoříme novou odstávku
                var newOdstavka = CreateNewOdstavka(lokalitaSearch, distrib, od, @do, popis);

                if(option=="hned")
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

                // Zavoláme dieslování (pokud je potřeba)
                result = await _dieslovaniService.HandleOdstavkyDieslovani(newOdstavka, result);
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = $"Neočekávaná chyba: {ex.Message}";
                return result;
            }
        }

        public async Task<HandleOdstavkyDieslovaniResult> TestOdstavkaAsync()
        {
            var result = new HandleOdstavkyDieslovaniResult();

            try
            {
                var number = await _odstavkaRepository.GetOdstavkaCountAsync();
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
                result = OdstavkyCheck(lokalitaSearch, od, do_, result);
                if (!result.Success)
                    return result;

                // Vytvoříme novou odstávku
                var newOdstavka = CreateNewOdstavka(lokalitaSearch, distrib, od, do_, popis);

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

        public async Task<Odstavka?> DetailOdstavkyAsync(int id)
        {
            return await _odstavkaRepository.GetByIdAsync(id);
        }

        public async Task<object> DetailOdstavkyJsonAsync(int id)
        {
            var detailOdstavky = await _odstavkaRepository.GetByIdAsync(id);
            if (detailOdstavky == null)
            {
                return new
                {
                    error = "Odstavka nenalezena"
                };
            }
            else if (detailOdstavky.Lokality == null)
            {
                return new
                {
                    error = "Odstavka nenalezena"
                };
            }
            else if (detailOdstavky.Lokality.Region == null)
            {
                return new
                {
                    error = "Odstavka nenalezena"
                };
            }

            return new
            {
                odstavkaId = detailOdstavky.ID,
                lokalita = detailOdstavky.Lokality.Nazev,
                adresa = detailOdstavky.Lokality.Adresa,
                klasifikace = detailOdstavky.Lokality.Klasifikace,
                baterie = detailOdstavky.Lokality.Baterie,
                region = detailOdstavky.Lokality.Region.Nazev,
                popis = detailOdstavky.Popis,
            };
        }

        public async Task<HandleOdstavkyDieslovaniResult> DeleteOdstavkaAsync(int idodstavky)
        {
            var result = new HandleOdstavkyDieslovaniResult();
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
        public async Task<(int totalRecords, List<object> data)> GetTableDataAsync(int start = 0, int length = 0)
        {
            var query = _odstavkaRepository.GetOdstavkaQuery();
            int totalRecords = query.Count();

            if (length == 0)
            {
                length = totalRecords;
            }

            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query.OrderBy(o => o.Od).Skip(start).Take(length));
            return (totalRecords, data.Cast<object>().ToList());
        }

        public async Task<List<object>> GetTableDataOdDetailAsync(int idodstavky)
        {
             var query = _odstavkaRepository.GetOdstavkaQuery()
                .Where(o => o.ID == idodstavky);
            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query);
            return data;
        }

        private static bool IsValidDateRange(DateTime od, DateTime Do)
        {
            return od.Date >= DateTime.Today && od < Do;
        }

        private bool ExistingOdstavka(int lokalitaSearchId, DateTime od)
        {
 
            var existingOdstavka =  _odstavkaRepository.AnotherOdsatvkaAsync(lokalitaSearchId,od);

            if (existingOdstavka == null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private HandleOdstavkyDieslovaniResult OdstavkyCheck(Lokalita lokalitaSearch, DateTime od, DateTime do_, HandleOdstavkyDieslovaniResult result)
        {
            if (!ExistingOdstavka(lokalitaSearch.ID, od))
            {
                result.Success = false;
                result.Message = "Již existuje jiná odstávka.";
                return result;
            }

            if (!IsValidDateRange(od, do_))
            {
                result.Success = false;
                result.Message = "Špatně zadané datum.";
                return result;
            }
            else
            {
                result.Success = true;
                return result;
            }
        }

        private Odstavka CreateNewOdstavka(Lokalita lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            var newOdstavka = new Odstavka
            {
                Distributor = distrib,
                Od = od,
                Do = do_,
                Popis = popis,
                Lokality = lokalitaSearch
            };
            return newOdstavka;
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

        public Task<HandleOdstavkyDieslovaniResult> UpdateOdstavkaAsync(int idodstavky, string lokalita, DateTime od, DateTime @do, string popis)
        {
            throw new NotImplementedException();
        }
       
    }
}