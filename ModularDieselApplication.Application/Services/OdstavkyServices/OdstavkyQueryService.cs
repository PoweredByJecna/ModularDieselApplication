using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Services;

namespace ModularDieselApplication.Application.Services
{
    public class OdstavkyQueryService
    {
        private readonly IOdstavkyRepository _odstavkaRepository;
        private readonly IDieslovaniService _dieslovaniService;

        public OdstavkyQueryService(IOdstavkyRepository odstavkaRepository, IDieslovaniService dieslovaniService)
        {
            _odstavkaRepository = odstavkaRepository;
            _dieslovaniService = dieslovaniService;
        }

        // ----------------------------------------
        // Suggest lokalita names based on a query.
        // ----------------------------------------
        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
            var lokalities = await _odstavkaRepository.GetAllAsync();

            return lokalities
                .Where(l => l.Nazev != null && l.Nazev.Contains(query))
                .Select(l => l.Nazev!)
                .Take(10)
                .ToList();
        }

        // ----------------------------------------
        // Get odstávka details as JSON.
        // ----------------------------------------
        public async Task<object> DetailOdstavkyJsonAsync(string id)
        {
            var detailOdstavky = await _odstavkaRepository.GetByIdAsync(id);

            if (detailOdstavky == null)
            {
                return new { error = "Odstavka nenalezena" };
            }
            else if (detailOdstavky.Lokality == null)
            {
                return new { error = "Lokalita nenalezena" };
            }
            else if (detailOdstavky.Lokality.Region == null)
            {
                return new { error = "Region nenalezen" };
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
                zacatekOdstavky = detailOdstavky.Od,
                konecOdstavky = detailOdstavky.Do
            };
        }

        // ----------------------------------------
        // Get table data for odstávky.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataAsync()
        {
            var query = _odstavkaRepository.GetOdstavkaQuery();

            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query.OrderBy(o => o.Od));
            return data;
        }

        // ----------------------------------------
        // Get detailed table data for a specific dieslovani.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataOdDetailAsync(string IDdieslovani)
        {
            var findodstavka = await _dieslovaniService.GetOdstavkaIDbyDieselId(IDdieslovani);
            var query = _odstavkaRepository.GetOdstavkaQuery()
                .Where(o => o.ID == findodstavka);
            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query);
            return data;
        }
    }
}