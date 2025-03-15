using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface IPohotovostiRepository
    {
        Task<List<Pohotovosti>> GetAllPohotovostiAsync();
        Task AddPohotovostAsync(Pohotovosti pohotovosti);
        Task<List<string>> GetPohotovostTechnikIdListAsync();
        Task<bool> GetPohotovostiRegionAsync(int idRegionu, DateTime OD, DateTime DO);
        Task<int> GetPohotovostCountAsync();
        Task<string> GetTechnikVPohotovostiAsnyc(int firmaid, DateTime OD, DateTime DO);
        Task<Technik> GetPohotovostTechnikIdsAsync(string id);
        Task<Dictionary<string, string>> GetTechnikLokalitaMapAsync(List<string> technikIds);
        Task<List<object>> GetPohotovostTableDataAsync(int start, int length, Dictionary<string, string> technikLokalitaMap);
    }
}