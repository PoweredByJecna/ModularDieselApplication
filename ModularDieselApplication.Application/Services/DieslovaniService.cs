using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Rules;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;


namespace ModularDieselApplication.Application.Services
{
    public class DieslovaniService(
        IDieslovaniRepository _dieslovaniRepository,
        IOdstavkyService _odstavkyService,
        ITechnikService _technikService,
        ILogService _logService,
        IPohotovostiRepository _pohotovostiService,
        DieslovaniRules _dieslovaniRules
        ) : IDieslovaniService
    {
        public async Task<bool> AnotherDieselRequestAsync(string idTechnika) => await _dieslovaniRepository.AnotherDieselRequest(idTechnika);
        public async Task<Dieslovani> GetDieslovani(GetDA filter, object value) => await _dieslovaniRepository.GetDaAsync(filter, value);
        public async Task<HandleResult> VstupAsync(string idDieslovani)
        {
            var dis = await _dieslovaniRepository.GetDaAsync(GetDA.ById, idDieslovani);
            if (dis != null)
            {
                dis.Nastav(DieslovaniFieldEnum.Vstup, DateTime.Now);
                dis.Technik.Nastav(TechnikFilterEnum.taken, true);
                await _dieslovaniRepository.UpdateAsync(dis);
                await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dis.ID, "Technik " + dis.Technik.User.Jmeno + " " + dis.Technik.User.Prijmeni + " vstoupil na lokalitu.");
                return HandleResult.OK("Vstup byl úspěšně zaznamenán.");
            }
            else return HandleResult.Error($"Dieslovani with id {idDieslovani} not found.");
        }
        public async Task<HandleResult> OdchodAsync(string idDieslovani)
        {
            var dis = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId, idDieslovani);
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
                return HandleResult.OK("Odchod byl úspěšně zaznamenán.");
            }
            else
            {
                return HandleResult.Error($"Dieslovani with id {idDieslovani} not found.");
            }
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
        public async Task<List<Dieslovani>> GetTableData(DieslovaniOdstavkaFilterEnum filter, User currentUser = null, bool isEngineer = default)
        {
            return filter switch
            {
                DieslovaniOdstavkaFilterEnum.AllTable => await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).ToListAsync(),
                DieslovaniOdstavkaFilterEnum.RunningTable => await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup != DateTime.MinValue && i.Odchod == DateTime.MinValue).ToListAsync(),
                DieslovaniOdstavkaFilterEnum.UpcomingTable => await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID != FiktivniTechnik.Id).ToListAsync(),
                DieslovaniOdstavkaFilterEnum.EndTable => await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Odchod != DateTime.MinValue.Date && i.Odstavka.Do.Date <= DateTime.Today).ToListAsync(),
                DieslovaniOdstavkaFilterEnum.TrashTable => await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID == FiktivniTechnik.Id).ToListAsync(),
                _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
            };
        }
        public async Task<HandleResult> ActionMethods(ActionFilter filter, string Id, DateTime time = default, User? currentUser = null)
        {
            try
            {
                return filter switch
                {
                    ActionFilter.Vstup => await VstupAsync(Id),
                    ActionFilter.Odchod => await OdchodAsync(Id),
                    ActionFilter.Delete => await DeleteDieslovaniAsync(Id),
                    ActionFilter.CallDA => await CallDieslovaniAsync(Id),
                    ActionFilter.ChangeTimeVstup => await ChangeTimeAsync(Id, time, ActionFilter.Vstup),
                    ActionFilter.ChangeTimeOdchod => await ChangeTimeAsync(Id, time, ActionFilter.Odchod),
                    ActionFilter.take => await TakeAsync(Id, currentUser),
                    _ => throw new ArgumentOutOfRangeException(nameof(filter), filter, null)
                };
            }
            catch
            {
                return HandleResult.Error($"An error occurred while processing the action: {filter} for ID: {Id}. Please check the logs for more details.");
            }
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        private async Task<HandleResult> CallDieslovaniAsync(string idodstavky)
        {
            var existingDieslovani = await AnotherDieselRequest(idodstavky);
            if (existingDieslovani) return HandleResult.Error($"Dieslovani for Odstavka with ID {idodstavky} already exists.");
            var odstavka = await _odstavkyService.GetOdstavka(GetOdstavka.ById, idodstavky);
            if (odstavka.Do.Date < DateTime.Now.Date) return HandleResult.Error($"Odstávka s ID {idodstavky} již skončila.");
            var technik = await _technikService.GetTechnik(GetTechnikEnum.ByUserId, FiktivniTechnik.Id);
            if (technik == null) return HandleResult.Error("Technik nebyl nalezen.");
            await CreateNewDieslovaniAsync(odstavka, technik);
            return HandleResult.OK("Dieslovani bylo úspěšně vytvořeno.");
        }
        private async Task SaveTechnikAndDieslovani(Dieslovani newdieslovani)
        {
            newdieslovani.Technik.Nastav(TechnikFilterEnum.taken, true);
            await _technikService.UpdateTechnikAsync(newdieslovani.Technik);
            await _dieslovaniRepository.UpdateAsync(newdieslovani);
        }
        private async Task<bool> AnotherDieselRequest(string odstavka) => await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId, odstavka) is not null;
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
        private async Task<HandleResult> TakeAsync(string idDieslovani, User currentUser)
        {
            var technik = await _technikService.GetTechnik(GetTechnikEnum.ByUserId, currentUser.Id);
            var dieslovaniTaken = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId, idDieslovani);
            var pohotovostTechnik = await _technikService.IsTechnikOnDutyAsync(technik.ID);
            if (!pohotovostTechnik) return HandleResult.Error("Technik není v pohotovosti.");
            dieslovaniTaken.Technik = technik;
            dieslovaniTaken.Technik.Nastav(TechnikFilterEnum.taken, true);
            await _technikService.UpdateTechnikAsync(dieslovaniTaken.Technik);
            await _dieslovaniRepository.UpdateAsync(dieslovaniTaken);
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovaniTaken.ID, $"Technik {dieslovaniTaken.Technik.User.Jmeno} {dieslovaniTaken.Technik.User.Prijmeni} si převzal lokalitu.");
            return HandleResult.OK("Dieslovani bylo úspěšně převzato.");
        }
        private async Task<HandleResult> DeleteDieslovaniAsync(string id)
        {
            _ = await _dieslovaniRepository.GetDaAsync(GetDA.ByOdstavkaId, id) ?? throw new InvalidOperationException($"Dieslovani with id {id} not found.");
            bool deleted = await _dieslovaniRepository.DeleteAsync(id);
            if (!deleted) throw new InvalidOperationException($"Failed to delete Dieslovani with id {id}.");
            return HandleResult.OK("Dieslovani was successfully deleted.");
        }
        private async Task<HandleResult> ChangeTimeAsync(string idDieslovani, DateTime time, ActionFilter type)
        {
            var dieslovani = await _dieslovaniRepository.GetDaAsync(GetDA.ById, idDieslovani) ?? throw new InvalidOperationException($"Dieslovani with id {idDieslovani} not found.");
            switch (type)
            {
                case ActionFilter.Vstup:
                    if (time.Date != dieslovani.Odstavka.Od.Date)
                    {
                        return HandleResult.Error("Vstup musí být v den odstávky.");
                    }
                    dieslovani.Nastav(DieslovaniFieldEnum.Vstup, time);
                    break;
                case ActionFilter.Odchod:
                    if (dieslovani.Vstup == DateTime.MinValue)
                    {
                        return HandleResult.Error("Nejprve musíte zadat vstup.");
                    }
                    dieslovani.Nastav(DieslovaniFieldEnum.Odchod, time);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
            await _dieslovaniRepository.UpdateAsync(dieslovani);
            await _logService.ZapisDoLogu(DateTime.Now, "Dieslovani", dieslovani.ID, $"Byl změnen čas na {time}.");
            return HandleResult.OK($"Čas byl úspěšně změněn na {time}.");
        }

    }
}

