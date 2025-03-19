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
        /* ----------------------------------------
            SuggestLokalitaAsync
        ---------------------------------------- */
        public async Task<List<string>> SuggestLokalitaAsync(string query)
        {
            var lokalities = await _odstavkaRepository.GetAllAsync();
            
            return [.. lokalities
                .Where(l => l.Nazev != null && l.Nazev.Contains(query))
                .Select(l => l.Nazev)
                .Take(10)];
        }
        /* ----------------------------------------
            DetailOdstavkyJsonAsync
        ---------------------------------------- */
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
                zacatekOdstavky = detailOdstavky.Od,
                konecOdstavky = detailOdstavky.Do

            };
        }
        /* ----------------------------------------
            DetailOdstavkyJsonAsync
        ---------------------------------------- */
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
        /* ----------------------------------------
            GetTableDataOdDetailAsync
        ---------------------------------------- */
        public async Task<List<object>> GetTableDataOdDetailAsync(int IDdieslovani)
        {
            var findodstavka = await _dieslovaniService.GetOdstavkaIDbyDieselId(IDdieslovani);
            var query = _odstavkaRepository.GetOdstavkaQuery()
                .Where(o => o.ID == findodstavka);
            var data = await _odstavkaRepository.GetOdstavkaDataAsync(query);
            return data;
        }

    }
}