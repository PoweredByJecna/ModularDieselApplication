using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Rules;
using static ModularDieselApplication.Application.Services.OdstavkyService;

namespace ModularDieselApplication.Application.Services
{
    public class DieslovaniService : IDieslovaniService
    {
        private readonly IDieslovaniRepository _dieslovaniRepository;
        private readonly IRegionyService _regionyService;
        private readonly ITechnikService _technikService;
        private readonly IEmailService _emailService;
        private readonly IPohotovostiService _pohotovostiService;
        private readonly IUserService _userService;
        

        public DieslovaniService(
            IDieslovaniRepository dieslovaniRepository,
            ITechnikService technikService,
            IRegionyService regionyService,
            IEmailService emailService,
            IPohotovostiService pohotovostiService,
            IUserService userService)
        {
            _dieslovaniRepository = dieslovaniRepository;
            _technikService = technikService;
            _regionyService = regionyService;
            _emailService = emailService;
            _pohotovostiService = pohotovostiService;
            _userService = userService;
        }

        /* ----------------------------------------
           GetTableDataAllTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery();
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }

        /* ----------------------------------------
           GetTableDataRunningTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup !=null);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }

        /* ----------------------------------------
           GetTableDataUpcomingTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataUpcomingTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(i => i.Vstup !=null && i.Odstavka.Od.Date==DateTime.Today);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }

        /* ----------------------------------------
           GetTableDataEndTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(User? currentUser, bool isEngineer)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.Odchod !=null && o.Vstup == null);
            query = FilteredData(query, currentUser, isEngineer);
            int totalRecords = query.Count();
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }

        /* ----------------------------------------
           GetTableDatathrashTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDatathrashTableAsync(User? currentUser, bool isEngineer)
        {
            if (currentUser == null)
            {
                throw new ArgumentNullException(nameof(currentUser), "Current user cannot be null.");
            }
            var technik = await _technikService.GetTechnikByUserIdAsync(currentUser.Id);

            if (technik == null || technik.Firma == null)
            {
                throw new InvalidOperationException("Technik or Firma is null.");
            }
            var firmaId = technik.Firma.ID;

            var validRegions = await _regionyService.GetRegionByIdFirmy(firmaId);

            var query = _dieslovaniRepository.GetDieslovaniQuery();

            if (isEngineer && validRegions.Any())
            {
                query = query.Where(d => validRegions.Contains(d.Odstavka.Lokality.Region.ID));
            }

            int totalRecords =  query.Count(); // Správné asynchronní počítání

            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return (totalRecords, data);
        }

        /* ----------------------------------------
           GetTableDataOdDetailOdstavkyAsync
           ---------------------------------------- */
        public async Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky)
        {
            var query = _dieslovaniRepository.GetDieslovaniQuery()
                .Where(o => o.ID == idodstavky);
            var data = await _dieslovaniRepository.GetDieslovaniDataAsync(query);
            return data;
        }

        /* ----------------------------------------
           HandleOdstavkyDieslovani
           ---------------------------------------- */
        public async Task<HandleOdstavkyDieslovaniResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleOdstavkyDieslovaniResult result)
        {
            if (newOdstavka?.Lokality?.DA == true)
            {
                result.Success = false;
                result.Message = "Na lokalitě není potřeba dieslovat, nachází se tam stacionární generátor.";
                return result;
            }
            if (newOdstavka?.Lokality?.Zasuvka == false)
            {
                result.Success = false;
                result.Message = "Na lokalitě se nedá dieslovat, protože tam není zásuvka.";
                return result;
            }
            if (newOdstavka != null && newOdstavka.Lokality != null && newOdstavka.Lokality.Klasifikace != null &&
                IsDieselRequired(newOdstavka.Lokality.Klasifikace, newOdstavka.Od, newOdstavka.Do, newOdstavka.Lokality.Baterie))
            {
                var technikSearch = await AssignTechnikAsync(newOdstavka);

                if (technikSearch == null)
                {
                    result.Success = false;
                    result.Message = "Nepodařilo se přiřadit technika.";
                    return result;
                }
                else
                {
                    var dieslovani = await CreateNewDieslovaniAsync(newOdstavka, technikSearch);
                    result.Success = true;

                    var EmailResult = "DA-ok";

                    await _emailService.SendDieslovaniEmailAsync(dieslovani, EmailResult);

                    result.Message = $"Dieslování č. {dieslovani.ID} bylo úspěšně vytvořeno.";
                }
            }
            else
            {
                result.Success = false;
                result.Message = "Dieslování není potřeba z důvodu" + result.Duvod;
                return result;
            }
            result.Success = true;
            return result;
        }

        /* ----------------------------------------
           IsDieselRequired
           ---------------------------------------- */
        private bool IsDieselRequired(string? klasifikace, DateTime Od, DateTime Do, int baterie)
        {
            var casVypadku = klasifikace.ZiskejCasVypadku();
            var rozdil = (Do - Od).TotalMinutes;

            if (casVypadku * 60 > rozdil)
            {
                return false;
            }
            else
            {
                if (Battery(Od, Do, baterie))
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }

        /* ----------------------------------------
           Battery
           ---------------------------------------- */
        private bool Battery(DateTime od, DateTime do_, int baterie)
        {
            var rozdil = (do_ - od).TotalMinutes;
            if (!int.TryParse(baterie.ToString(), out var baterieMinuty))
                baterieMinuty = 0;

            return rozdil <= baterieMinuty;
        }

        /* ----------------------------------------
           AssignTechnikAsync
           ---------------------------------------- */
        private async Task<Technik?> AssignTechnikAsync(Odstavka newOdstavka)
        {
            var firmaVRegionu = await GetFirmaVRegionuAsync(newOdstavka.Lokality.Region.ID);

            if (firmaVRegionu != null)
            {
                // vratí technika, který je v zapsán v pohotovosti, a mám status taken = false
                var technikSearch = await _pohotovostiService.GetTechnikActivTechnikByIdFirmaAsync(firmaVRegionu.ID);


                if (technikSearch == null)
                {
                    bool nejakyTechnikMaPohotovost = await _pohotovostiService.PohovostiVRegionuAsync(firmaVRegionu.ID);

                    if (nejakyTechnikMaPohotovost)
                    {
                        technikSearch = await CheckTechnikReplacementAsync(newOdstavka);
                        if (technikSearch != null)
                        {
                            return technikSearch;
                        }
                    }
                    var fiktivniTechnik = await _technikService.GetTechnikByIdAsync("606794494");
                }

                return technikSearch;
            }
            else
            {
                return null;
            }
        }

        /* ----------------------------------------
           GetFirmaVRegionuAsync
           ---------------------------------------- */
        private async Task<Firma?> GetFirmaVRegionuAsync(int regionId)
        {
            return await _technikService.GetFirmaVRegionuAsync(regionId);
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

            if (dieslovani.Odstavka.Do < newOdstavka.Od.AddHours(3) || newOdstavka.Do < dieslovani.Odstavka.Od.AddHours(3))
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

        /* ----------------------------------------
           VstupAsync
           ---------------------------------------- */
        public async Task<(bool Success, string Message)> VstupAsync(int idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null)
                {
                    dis.Vstup = DateTime.Now;
                    dis.Technik.Taken = true;
    
                    await _dieslovaniRepository.UpdateAsync(dis);
                    return (true, "Byl zadán vstup na lokalitu.");
                }
                else
                {
                    return (false, "Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při zadávání vstupu " + ex.Message);
            }
        }

        /* ----------------------------------------
           OdchodAsync
           ---------------------------------------- */
        public async Task<(bool Success, string Message)> OdchodAsync(int idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dis != null)
                {              
                    var anotherDiesel = await _dieslovaniRepository.AnotherDieselRequest(dis.Technik.ID);


                    if (anotherDiesel == false)
                    {
                        dis.Technik.Taken = false;
                        await _technikService.UpdateTechnikAsync(dis.Technik);
                    }

                    dis.Odchod = DateTime.Now;
                    await _dieslovaniRepository.UpdateAsync(dis);

                    return (true, "Byl zadán odchod z lokality.");
                }
                else
                {
                    return (false, "Záznam dieslování nebyl nalezen.");
                }
            }
            catch (Exception ex)
            {
                return (false, "Chyba při zadávání odchodu " + ex.Message);
            }
        }

        /* ----------------------------------------
           TemporaryLeaveAsync
           ---------------------------------------- */
        public async Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani)
        {
            try
            {
                var dis = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                dis.Technik.Taken = !dis.Technik.Taken;

                await _dieslovaniRepository.UpdateAsync(dis);
                return (true, $"Změněn stav technika (Taken = {!dis.Technik.Taken}).");
            }
            catch (Exception ex)
            {
                return (false, "Chyba při dočasném uvolnění: " + ex.Message);
            }
        }

        /* ----------------------------------------
           TakeAsync
           ---------------------------------------- */
        public async Task<(bool Success, string Message, string? TempMessage)> TakeAsync(int idDieslovani, User currentUser)
        {
            try
            {
                var technik = await _technikService.GetTechnikByIdAsync(currentUser.Id);

                var dieslovaniTaken = await _dieslovaniRepository.GetByIdAsync(idDieslovani);

                if (dieslovaniTaken == null)
                {
                    return (false, "Záznam dieslování nebyl nalezen.", null);
                }

                var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);

                if (!pohotovostTechnik)
                {
                    return (false, "Nejste zapsán v pohotovosti.", null);
                }

                if (technik.Taken)
                {
                    return (false, "Již máte převzaté jiné dieslování.", null);
                }

                dieslovaniTaken.Technik = technik;
                technik.Taken = true;
                await _technikService.UpdateTechnikAsync(technik);
                await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);

                return (true, $"Lokalitu si převzal: {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Jmeno}", 
                             "Dieslování bylo úspěšně zadáno.");
            }
            catch (Exception ex)
            {
                return (false, $"Chyba při převzetí: {ex.Message}", null);
            }
        }

        /* ----------------------------------------
           DetailDieslovaniAsync
           ---------------------------------------- */
        public async Task<Dieslovani?> DetailDieslovaniAsync(int id)
        {
            return await _dieslovaniRepository.GetByIdAsync(id);
        }

        /* ----------------------------------------
           DetailDieslovaniJsonAsync
           ---------------------------------------- */
        public async Task<object> DetailDieslovaniJsonAsync(int id)
        {
            var detailDieslovani = await _dieslovaniRepository.GetByIdAsync(id);
            if (detailDieslovani == null)
            {
                return new
                {
                    error = "Dieslovani nenalezena"
                };
            }

            return new
            {
                dieslovaniId = detailDieslovani.ID,
                vstup = detailDieslovani.Vstup,
                odchod = detailDieslovani.Odchod,
                odstavka = new
                {
                    detailDieslovani.Odstavka.ID,
                    detailDieslovani.Odstavka.Od,
                    detailDieslovani.Odstavka.Do,
                    detailDieslovani.Odstavka.Popis,
                    detailDieslovani.Odstavka.Distributor,
                    lokalita = new
                    {
                        detailDieslovani.Odstavka.Lokality.Nazev,
                        detailDieslovani.Odstavka.Lokality.Adresa,
                        detailDieslovani.Odstavka.Lokality.Klasifikace,
                        detailDieslovani.Odstavka.Lokality.Baterie,
                        region = new
                        {
                            detailDieslovani.Odstavka.Lokality.Region.Nazev
                        }
                    }
                },
                firma = new
                {
                    detailDieslovani.Technik.Firma.Nazev
                },
                technik = new
                {
                    detailDieslovani.Technik.User.Jmeno,
                    detailDieslovani.Technik.User.Prijmeni,
                    detailDieslovani.Technik.User.Email
                }
            };
        }
        public async Task<Dieslovani> CreateNewDieslovaniAsync(Odstavka newOdstavka, Technik technik)
        {
        var newDieslovani = new Dieslovani
        {
            Vstup = DateTime.MinValue,
            Odchod = DateTime.MinValue,
            Odstavka=newOdstavka,
            Technik = technik
        };
        await _dieslovaniRepository.AddAsync(newDieslovani);
        technik.Taken = true;
        await _technikService.UpdateTechnikAsync(technik);

        return newDieslovani;
        }

        /* ----------------------------------------
           DeleteDieslovaniJsonAsync
           ---------------------------------------- */
       public async Task<(bool Success, string Message)> DeleteDieslovaniAsync(int idDieslovani)
    {
            try
            {
                var dieslovani = await _dieslovaniRepository.GetByIdAsync(idDieslovani);
                if (dieslovani == null)
                {
                    return (false, "Dieslovani nebylo nalezeno.");
                }

                bool deleted = await _dieslovaniRepository.DeleteAsync(idDieslovani);
                if (!deleted)
                {
                    return (false, "Dieslovani se nepodařilo smazat.");
                }

                return (true, "Dieslovani byla úspěšně smazána.");
            }
            catch (Exception ex)
            {
                return (false, $"Chyba při mazání dieslovani: {ex.Message}");
            }
        }

      
        /* ----------------------------------------
           FilteredData
           ---------------------------------------- */
        private static IQueryable<Dieslovani> FilteredData(
            IQueryable<Dieslovani> query,
            User? currentUser,
            bool isEngineer)
        {
            if (currentUser == null)
                return query;

            if (isEngineer)
            {
                var userId = currentUser.Id;
                query = query.Where(d => d.Technik.User.Id == userId);
            }
            return query;
        }
        public async Task<Dieslovani> GetDieslovaniByOdstavkaId(int id)
        {
            return await _dieslovaniRepository.GetDAbyOdstavkaAsync(id);
        }

      
    }    
}

       