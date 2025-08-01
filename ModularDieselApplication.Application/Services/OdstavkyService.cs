using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Enum;

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
        public async Task DeleteOdstavkaAsync(string idodstavky)
        {
            var odstavka = await _odstavkaRepository.GetOdstavkaAsync(Domain.Enum.GetOdstavka.ById, idodstavky);
            if (odstavka == null)
            {
                throw new InvalidOperationException("Záznam nebyl nalezen");
            }
            await _odstavkaRepository.DeleteAsync(idodstavky);
        }
        public async Task ChangeTimeOdstavkyAsync(string idodstavky, DateTime time, ActionFilter filter)
        {
            var odstavka = await _odstavkaRepository.GetOdstavkaAsync(Domain.Enum.GetOdstavka.ById, idodstavky);
            switch (filter)
            {
                case ActionFilter.zacatek:
                    if (odstavka.Od.Date < DateTime.Today.Date)
                    {
                        throw new InvalidOperationException("Nelze měnit čas již ukončené odstávky.");
                    }
                    else if (odstavka.Do.Date < time.Date)
                    {
                        throw new InvalidOperationException("Začátek odstávky nesmí být později než konec.");
                    }
                    odstavka.Od = time;
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", odstavka.ID, $"Byl změněn čas začátku odstávky na: {odstavka.Od}");
                    await _odstavkaRepository.UpdateAsync(odstavka);

                    break;
                case ActionFilter.konec:
                    if (odstavka.Do.Date < DateTime.Today.Date)
                    {
                        throw new InvalidOperationException("Nelze měnit čas již ukončené odstávky.");
                    }
                    else if (odstavka.Od.Date > time.Date)
                    {
                        throw new InvalidOperationException("Konec odstávky nesmí být dříve než začátek.");
                    }
                    odstavka.Do = time;
                    await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", odstavka.ID, $"Byl změněn čas konce odstávky na: {odstavka.Do}");
                    await _odstavkaRepository.UpdateAsync(odstavka);

                    break;
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
        // Create a new odstávka record.
        // ----------------------------------------
        public async Task<Odstavka> CreateNewOdstavka(Lokalita lokalitaSearch, string distrib, DateTime od, DateTime do_, string popis)
        {
            var newOdstavka = new Odstavka
            {
                Distributor = distrib,
                Od = od,
                Do = do_,
                Popis = popis,
                Lokality = lokalitaSearch
            };
            await _odstavkaRepository.AddAsync(newOdstavka);

            await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Nová odstávka č.{newOdstavka.ID} bylo vytvořeno.");
            await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Vytvřáření odstávky s parametry: Lokalita: {newOdstavka.Lokality.Nazev}, Klasifikace: {newOdstavka.Lokality.Klasifikace}, Od: {newOdstavka.Od}, Do: {newOdstavka.Do}");
            await _logService.ZapisDoLogu(DateTime.Now, "Odstávka", newOdstavka.ID, $"Baterie: {newOdstavka.Lokality.Baterie} min");
            return newOdstavka;
        
           
        }
        

    }
}