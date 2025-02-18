using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Application.Services
{
    public class PohotovostiService : IPohotovostiService
    {
        private readonly IPohotovostiRepository _pohotovostiRepository;
        private readonly IUserService _userService;

        private readonly ITechnikService _technikRepository;

        public PohotovostiService(IPohotovostiRepository pohotovostiRepository, IUserService userService, ITechnikService technikRepository)
        {
            _pohotovostiRepository = pohotovostiRepository;
            _userService = userService;
            _technikRepository = technikRepository;
        }

        public async Task<List<Pohotovosti>> GetAllPohotovostiAsync()
        {
            return await _pohotovostiRepository.GetAllPohotovostiAsync();
        }
        public async Task<bool> PohovostiVRegionuAsync(int idRegionu)
        {
            return await _pohotovostiRepository.GetPohotovostiRegionAsync(idRegionu);
        }

        public async Task<(bool Success, string Message)> ZapisPohotovostAsync(Pohotovosti pohotovosti, User currentUser)
        {
            bool isEngineer = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Engineer");
            bool isAdmin = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Admin");

            if (!isEngineer && !isAdmin)
            {
                return (false, "Nemáte oprávnění zapsat pohotovost.");
            }

            if (pohotovosti.Zacatek <= pohotovosti.Zacatek || pohotovosti.Zacatek < DateTime.Today)
            {
                return (false, "Neplatný interval pohotovosti.");
            }

            if (isEngineer)
            {
                if (currentUser == null)
                {
                    return (false, "Aktuální uživatel je null.");
                }

                var technikSearch = await _pohotovostiRepository.GetPohotovostTechnikIdsAsync(currentUser.Id);

                if (technikSearch == null)
                {
                    return (false, "Nepodařilo se najít technika přiřazeného k aktuálnímu uživateli.");
                }

                var zapis = new Pohotovosti
                {
                    IdUser = technikSearch.User.Id,
                    Zacatek = pohotovosti.Zacatek,
                    Konec = pohotovosti.Konec,
                    IdTechnik = technikSearch.ID
                };

                await _pohotovostiRepository.AddPohotovostAsync(zapis);

                return (true, "Pohotovost byla úspěšně zapsána (engineer).");
            }

            if (isAdmin)
            {
                var technikSearch = await _pohotovostiRepository.GetPohotovostTechnikIdsAsync(pohotovosti.Technik.ID);

                if (technikSearch == null)
                {
                    return (false, "Nepodařilo se najít technika podle zadaného IdTechnika.");
                }

                var zapis = new Pohotovosti
                {
                    IdUser = technikSearch.User.Id,
                    Zacatek = pohotovosti.Zacatek,
                    Konec = pohotovosti.Konec,
                    IdTechnik = technikSearch.ID
                };

                await _pohotovostiRepository.AddPohotovostAsync(zapis);

                return (true, "Pohotovost byla úspěšně zapsána (admin).");
            }

            return (false, "Nepodařilo se provést zápis pohotovosti.");
        }

        public async Task<(int totalRecords, List<object> data)> GetPohotovostTableDataAsync(int start, int length)
        {
            int totalRecords = await _pohotovostiRepository.GetPohotovostCountAsync();
            if (length == 0)
            {
                length = totalRecords;
            }

            var pohotovostTechnikIds = await _pohotovostiRepository.GetPohotovostTechnikIdListAsync();
            var technikLokalitaMap = await _pohotovostiRepository.GetTechnikLokalitaMapAsync(pohotovostTechnikIds);

            var pohotovostList = await _pohotovostiRepository.GetPohotovostTableDataAsync(start, length, technikLokalitaMap);

            return (totalRecords, pohotovostList);
        }
        public async Task<Technik> GetTechnikActivTechnikByIdFirmaAsync(int idFirma)
        {
            var technik = await _technikRepository.GetTechnikByIdFrimy(idFirma);

            return technik;
        }
        

    }
}