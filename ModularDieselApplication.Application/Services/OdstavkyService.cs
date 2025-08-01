using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;
using Microsoft.EntityFrameworkCore;

namespace ModularDieselApplication.Application.Services
{
    public class OdstavkyService : IOdstavkyService
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly ILokalityService _lokalityService;
        private readonly ILogService _logService;

        public OdstavkyService(IOdstavkyRepository odstavkaRepository, ILokalityService lokalityService, ILogService logService)
        {
            _odstavkaRepository = odstavkaRepository;
                _lokalityService = lokalityService;
                _logService = logService;
        }

        public async Task<HandleResult> ActionMethods(ServiceFilterEnum serviceFilter, ActionFilter filter, string Id, DateTime time = default, User? currentUser = null)
        {
            return filter switch
                {
                    ActionFilter.ChangeTimeZactek => await ChangeTimeOdstavkyAsync(Id, time, ActionFilter.zacatek),
                    ActionFilter.ChangeTimeKonec => await ChangeTimeOdstavkyAsync(Id, time, ActionFilter.konec),
                    ActionFilter.Delete => await DeleteOdstavkaAsync(Id),
                    _ => throw new NotImplementedException("Odstavky action filter is not implemented yet.")
                };
        }
        public async Task<List<Odstavka>> GetTableData() => await _odstavkaRepository.GetOdstavkaQuery().ToListAsync();

        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
            var lokalities = await _lokalityService.GetAllLokalityAsync();
            var result = lokalities
            .Where(l => l.Nazev.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Select(l => l.Nazev)
            .ToList();
            return result;
        }
        public async Task<Odstavka> GetOdstavka(GetOdstavka filter, object value)
        {
            var odstavka = await _odstavkaRepository.GetOdstavkaAsync(filter, value);
            return odstavka;
        }
        private async Task<HandleResult> DeleteOdstavkaAsync(string idodstavky)
        {
            var odstavka = await _odstavkaRepository.GetOdstavkaAsync(Domain.Enum.GetOdstavka.ById, idodstavky);
            if (odstavka == null)
            {
                return HandleResult.Error("Záznam nebyl nalezen");
            }
            await _odstavkaRepository.DeleteAsync(idodstavky);
            return HandleResult.OK("Záznam byl úspěšně smazán.");
        }
        private async Task<HandleResult> ChangeTimeOdstavkyAsync(string idodstavky, DateTime time, ActionFilter filter)
        {
            var odstavka = await _odstavkaRepository.GetOdstavkaAsync(Domain.Enum.GetOdstavka.ById, idodstavky);
            switch (filter)
            {
                case ActionFilter.zacatek:
                    if (odstavka.Od.Date < DateTime.Today.Date)
                    {
                        return HandleResult.Error("Nelze měnit čas již ukončené odstávky.");
                    }
                    else if (odstavka.Do.Date < time.Date)
                    {
                        return HandleResult.Error("Začátek odstávky nesmí být později než konec.");
                    }
                    odstavka.Od = time;
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", odstavka.ID, $"Byl změněn čas začátku odstávky na: {odstavka.Od}");
                    await _odstavkaRepository.UpdateAsync(odstavka);
                    return HandleResult.OK("Čas začátku byl úspěšně změněn.");

                case ActionFilter.konec:
                    if (odstavka.Do.Date < DateTime.Today.Date)
                    {
                        return HandleResult.Error("Nelze měnit čas již ukončené odstávky.");
                    }
                    else if (odstavka.Od.Date > time.Date)
                    {
                        return HandleResult.Error("Konec odstávky nesmí být dříve než začátek.");
                    }
                    odstavka.Do = time;
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", odstavka.ID, $"Byl změněn čas konce odstávky na: {odstavka.Do}");
                    await _odstavkaRepository.UpdateAsync(odstavka);
                    return HandleResult.OK("Čas konce byl úspěšně změněn.");

                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }
        
         private async Task<bool> ExistingOdstavka(string lokalitaSearchId, DateTime od)
        {
            var existingOdstavka = await _odstavkaRepository.AnotherOdsatvkaAsync(lokalitaSearchId, od);

            return existingOdstavka == null;
        }

        // ----------------------------------------
        // Determine the distributor based on the region name.
        // ----------------------------------------
        private string DetermineDistributor(string NazevRegionu)
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
        // Create a new odstávka record.
        // ----------------------------------------
        public async Task<HandleResult> CreateNewOdstavka(string lokalita, DateTime od, DateTime do_, string popis)
        {
            var lokalitaSearch = await _lokalityService.GetLokalita(GetLokalita.ByNazev,lokalita);
            var newOdstavka = new Odstavka
            {
                Distributor = DetermineDistributor(lokalitaSearch.Region.Nazev),
                Od = od,
                Do = do_,
                Popis = popis,
                Lokality = lokalitaSearch
            };
            await _odstavkaRepository.AddAsync(newOdstavka);

            await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Nová odstávka č.{newOdstavka.ID} bylo vytvořeno.");
            await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Vytvřáření odstávky s parametry: Lokalita: {newOdstavka.Lokality.Nazev}, Klasifikace: {newOdstavka.Lokality.Klasifikace}, Od: {newOdstavka.Od}, Do: {newOdstavka.Do}");
            await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Baterie: {newOdstavka.Lokality.Baterie} min");
            return HandleResult.OK("");
        }
        

    }
}