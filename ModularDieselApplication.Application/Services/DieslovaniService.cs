using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Rules;
using Microsoft.EntityFrameworkCore;


namespace ModularDieselApplication.Application.Services
{
    public class DieslovaniService : IDieslovaniService
    {
        private readonly IDieslovaniRepository _dieslovaniRepository;
        private readonly ITechnikService _technikService;
        private readonly ILogService _logService;
        private readonly IPohotovostiRepository _pohotovostiService;
        private readonly DieslovaniRules _dieslovaniRules;
        private readonly IOdstavkyService _odstavkyService;

        public DieslovaniService(
            IDieslovaniRepository dieslovaniRepository,
            IOdstavkyService odstavkyService,
            ITechnikService technikService,
            ILogService logService,
            IPohotovostiRepository pohotovostiService,
            DieslovaniRules dieslovaniRules
        )
        {
            _dieslovaniRepository = dieslovaniRepository;
            _technikService = technikService;
            _logService = logService;
            _pohotovostiService = pohotovostiService;
            _dieslovaniRules = dieslovaniRules;
            _odstavkyService = odstavkyService;
        }
        public async Task<bool> AnotherDieselRequestAsync(string idTechnika)
        {
            return await _dieslovaniRepository.AnotherDieselRequest(idTechnika);
        }
        public async Task<Dieslovani> GetDieslovani(GetDA filter, object value)
        {
            Dieslovani dieslovani = await _dieslovaniRepository.GetDaAsync(filter,value);
            return dieslovani;
        } 

        public async Task VstupAsync(string idDieslovani)
        {
            var dis = await _dieslovaniRepository.GetDaAsync(GetDA.ById, idDieslovani);

            if (dis != null)
            {
                dis.Nastav(DieslovaniFieldEnum.Vstup, DateTime.Now);
                dis.Technik.Nastav(TechnikFilterEnum.taken, true);
                await _dieslovaniRepository.UpdateAsync(dis);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " vstoupil na lokalitu.");
            }
            else
            {
                throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            }
        }
        // ----------------------------------------
        // Record exit from a location.
        // ----------------------------------------
        public async Task OdchodAsync(string idDieslovani)
        {
            var dis = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId,idDieslovani);
            if (dis != null)
            {
                var anotherDiesel = await _dieslovaniRepository.AnotherDieselRequest(dis.Technik.ID);

                if (!anotherDiesel)
                {
                    dis.Technik.Nastav(TechnikFilterEnum.taken, false);
                    await _technikService.UpdateTechnikAsync(dis.Technik);
                }

                dis.Nastav(DieslovaniFieldEnum.Odchod, DateTime.Now);
                await _dieslovaniRepository.UpdateAsync(dis);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " zadal odchod z lokality.");
            }
            else
            {
                throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            }
        }
        // ----------------------------------------
        // Assign a technician to a location.
        // ----------------------------------------
        public async Task TakeAsync(string idDieslovani, User currentUser)
        {

            var technik = await _technikService.GetTechnik(GetTechnikEnum.ByUserId, currentUser.Id);
            var dieslovaniTaken = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId,idDieslovani);
            var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);
            if (!pohotovostTechnik)
            {
                throw new InvalidOperationException("Technik není v pohotovosti.");
            }
            dieslovaniTaken.Technik = technik;
            dieslovaniTaken.Technik.Nastav(TechnikFilterEnum.taken, true);
            await _technikService.UpdateTechnikAsync(dieslovaniTaken.Technik);
            await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovaniTaken.ID, $"Technik {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni} si převzal lokalitu.");
        }

        // ----------------------------------------
        // Delete a dieslovani record.
        // ----------------------------------------
        public async Task DeleteDieslovaniAsync(string id)
        {
            var dieslovani = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId,id);
            if (dieslovani == null)
            {
                throw new InvalidOperationException($"Dieslovani with id {id} not found.");
            }
            bool deleted = await _dieslovaniRepository.DeleteAsync(id);
            if (!deleted)
            {
                throw new InvalidOperationException($"Failed to delete Dieslovani with id {id}.");
            }
        }

        // ----------------------------------------
        // Change the time for a dieslovani record.
        // ----------------------------------------
        public async Task ChangeTimeAsync(string idDieslovani, DateTime time, ActionFilter type)
        {
            var dieslovani = await _dieslovaniRepository.GetDaAsync(GetDA.ById,idDieslovani) ?? throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            switch (type)
            {
                case ActionFilter.Vstup:
                    if (time.Date != dieslovani.Odstavka.Od.Date)
                    {
                        throw new InvalidOperationException("Vstup musí být v den odstávky.");
                    }
                    dieslovani.Nastav(DieslovaniFieldEnum.Vstup, time);
                    break;
                case ActionFilter.Odchod:
                    if (dieslovani.Vstup == DateTime.MinValue)
                    {
                        throw new InvalidOperationException("Nejprve musíte zadat vstup.");
                    }
                    dieslovani.Nastav(DieslovaniFieldEnum.Odchod, time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            await _dieslovaniRepository.UpdateAsync(dieslovani);
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Byl změnen čas na {time}.");
        }

        public async Task<HandleResult<Dieslovani>> HandleOdstavkyDieslovani(Odstavka newOdstavka)
        {
            var technik = await _technikService.GetTechnik(GetTechnikEnum.fitkivniTechnik);

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
                        return HandleResult<Dieslovani>.OK(dieslovani, $"Odstávka č. {newOdstavka.ID}, byla vytvořena.\nBylo vytvořeno nové dieslování č.{dieslovani.ID}.\nTechnik: {dieslovani.Technik.User.Jmeno} {dieslovani.Technik.User.Jmeno}.");
                }
            }

        }

        // ----------------------------------------
        // Assign a technician to dieslovani.
        // ----------------------------------------
        public async Task<Technik> AssignTechnikAsync(Dieslovani dieslovani)
        {
            var techniksearch = await _pohotovostiService.GetTechnikVPohotovostiAsnyc(dieslovani.Odstavka.Lokality.Region.ID, dieslovani.Odstavka.Od, dieslovani.Odstavka.Do);
            if (techniksearch.ID != FiktivniTechnik.Id)
            {
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", dieslovani.ID, $"Firma která bude zajišťovat dieslování: {dieslovani.Odstavka.Lokality.Region.Firma.Nazev} a její technik {techniksearch}.");
                techniksearch.Nastav(TechnikFilterEnum.taken, true);
                await _technikService.UpdateTechnikAsync(techniksearch);
                return techniksearch;
            }
            else
            {
                techniksearch = await CheckTechnikReplacementAsync(dieslovani);
                if (techniksearch.ID == FiktivniTechnik.Id)
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", dieslovani.ID, $"Technika se nepodařilo najít, je přiřazen: {techniksearch}.");
                    return techniksearch;
                }
                else
                {
                    await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", dieslovani.ID, $"Technik byl úspěšně přiřazen: {techniksearch}.");
                    return techniksearch;
                }
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
            newDieslovani.Technik = await AssignTechnikAsync(newDieslovani);
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovaní", newDieslovani.ID, $"Nové dieslování č.{newDieslovani.ID} bylo vytvořeno.");
            return newDieslovani;
        }

        // ----------------------------------------
        // Save technician and dieslovani details.
        // ----------------------------------------
        private async Task SaveTechnikAndDieslovani(Dieslovani newdieslovani)
        {
            newdieslovani.Technik.Nastav(TechnikFilterEnum.taken, true);
            await _technikService.UpdateTechnikAsync(newdieslovani.Technik);
            await _dieslovaniRepository.UpdateAsync(newdieslovani);
        }

        // ----------------------------------------
        // Call dieslovani for an odstávka.
        // ----------------------------------------
        public async Task CallDieslovaniAsync(string idodstavky)
        {
            var existingDieslovani = await AnotherDieselRequest(idodstavky);
            if (existingDieslovani)
            {
                throw new InvalidOperationException($"Dieslovani for Odstavka with ID {idodstavky} already exists.");
            }
            var odstavka = await _odstavkyService.GetOdstavka(GetOdstavka.ById,idodstavky);

            if (odstavka.Do.Date < DateTime.Now.Date)
            {
                throw new InvalidOperationException($"Odstávka s ID {idodstavky} již skončila.");
            }
            var technik = await _technikService.GetTechnik(GetTechnikEnum.ByUserId, FiktivniTechnik.Id);

            if (technik == null)
            {
                throw new InvalidOperationException("Technik nebyl nalezen.");
            }
            await CreateNewDieslovaniAsync(odstavka, technik);
        }

        // ----------------------------------------
        // Check if another dieslovani request exists.
        // ----------------------------------------
        private async Task<bool> AnotherDieselRequest(string odstavka)
        {
            var dieslovani = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId, odstavka);
            return dieslovani != null;
        }

          // ----------------------------------------
        // Check for a technician replacement.
        // ----------------------------------------
        private async Task<Technik> CheckTechnikReplacementAsync(Dieslovani dieslovani)
        {
            List<Dieslovani> anotherDieslovani = await _dieslovaniRepository.GetAnotherDA(dieslovani);
            int novaVaha;
            int staraVaha;
            bool maVyssiPrioritu;
            bool casovyLimit;
            foreach (var stareDieslovani in anotherDieslovani)
            {
                if (stareDieslovani.Odstavka.Do < dieslovani.Odstavka.Od.AddHours(3) || stareDieslovani.Odstavka.Do < dieslovani.Odstavka.Od.AddHours(4))
                {
                    dieslovani.Technik = stareDieslovani.Technik;
                    await SaveTechnikAndDieslovani(dieslovani);
                    return dieslovani.Technik;
                }
                novaVaha = stareDieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
                staraVaha = dieslovani.Odstavka.Lokality.Klasifikace.ZiskejVahu();
                maVyssiPrioritu = novaVaha > staraVaha;
                casovyLimit = dieslovani.Odstavka.Od.Date.AddHours(3) > DateTime.Now.Date;
                if (maVyssiPrioritu && casovyLimit)
                {
                    dieslovani.Technik = stareDieslovani.Technik;
                    await SaveTechnikAndDieslovani(dieslovani);
                    stareDieslovani.Technik = await _technikService.GetTechnik(GetTechnikEnum.fitkivniTechnik);
                    await SaveTechnikAndDieslovani(stareDieslovani);
                    return dieslovani.Technik;
                }
                else return dieslovani.Technik;
            }
            return dieslovani.Technik;
        }
        
        
            

    }
}

