using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.Extensions.Diagnostics.Metrics;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Domain.Objects;

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

        // ----------------------------------------
        // Get all pohotovosti records.
        // ----------------------------------------
        public async Task<List<Pohotovosti>> GetAllPohotovostiAsync()
        {
            return await _pohotovostiRepository.GetAllPohotovostiAsync();
        }

        // ----------------------------------------
        // Check if pohotovosti exist in a specific region within a time range.
        // ----------------------------------------
        public async Task<bool> PohovostiVRegionuAsync(string idRegionu, DateTime OD, DateTime DO)
        {
            return await _pohotovostiRepository.GetPohotovostiRegionAsync(idRegionu, OD, DO);
        }

        // ----------------------------------------
        // Record a new pohotovost for a user.
        // ----------------------------------------
        public async Task<HandleResult> ZapisPohotovostAsync(DateTime zacatek, DateTime konec, User currentUser)
        {
            var result = new HandleResult();
            bool isEngineer = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Engineer");
            bool isAdmin = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Admin");

            if (!isEngineer && !isAdmin)
            {
                result.Success = false;
                result.Message = "Nemáte oprávnění zapsat pohotovost.";
                return result;
            }

            if (zacatek.Date > konec.Date)
            {
                result.Success = false;
                result.Message = "Neplatný interval pohotovosti.";
                return result;
            }

            if (isEngineer)
            {
                if (currentUser == null)
                {
                    result.Success = false;
                    result.Message = "Aktuální uživatel je null.";
                    return result;
                }

                var technikSearch = await _technikRepository.GetTechnikByUserIdAsync(currentUser.Id);

                if (technikSearch == null)
                {
                    result.Success = false;
                    result.Message = "Nepodařilo se najít technika přiřazeného k aktuálnímu uživateli.";
                    return result;
                }

                var zapis = new Pohotovosti
                {
                    ID = Guid.NewGuid().ToString(),
                    IdUser = technikSearch.User.Id,
                    User = technikSearch.User,
                    Zacatek = zacatek,
                    Konec = konec,
                    IdTechnik = technikSearch.ID,
                    Technik = technikSearch
                };

                await _pohotovostiRepository.AddPohotovostAsync(zapis);

                result.Success = true;
                result.Message = "Pohotovost byla úspěšně zapsána.";
                return result;
            }

            if (isAdmin)
            {
                if (currentUser == null)
                {
                    result.Success = false;
                    result.Message = "Aktuální uživatel je null.";
                    return result;
                }
                
                var technikSearch = await _technikRepository.GetTechnikByUserIdAsync(currentUser.Id);

                if (technikSearch == null)
                {
                    result.Success = false;
                    result.Message = "Nepodařilo se najít technika podle zadaného IdTechnika.";
                    return result;
                }

                var zapis = new Pohotovosti
                {
                    IdUser = technikSearch.User.Id,
                    Zacatek = zacatek,
                    Konec = konec,
                    User = technikSearch.User,
                    Technik = technikSearch,
                    IdTechnik = technikSearch.ID
                };

                await _pohotovostiRepository.AddPohotovostAsync(zapis);

                result.Success = true;
                result.Message = "Pohotovost byla úspěšně zapsána.";
                return result;
            }

            result.Success = false;
            result.Message = "Nepodařilo se provést zápis pohotovosti.";
            return result;
        }

        // ----------------------------------------
        // Get table data for pohotovosti.
        // ----------------------------------------
        public async Task<List<object>> GetPohotovostTableDataAsync(int start, int length)
        {
            int totalRecords = await _pohotovostiRepository.GetPohotovostCountAsync();
            if (length == 0)
            {
                length = totalRecords;
            }

            var pohotovostTechnikIds = await _pohotovostiRepository.GetPohotovostTechnikIdListAsync();
            var technikLokalitaMap = await _pohotovostiRepository.GetTechnikLokalitaMapAsync(pohotovostTechnikIds);

            var pohotovostList = await _pohotovostiRepository.GetPohotovostTableDataAsync(start, length, technikLokalitaMap);

            return pohotovostList;
        }

        // ----------------------------------------
        // Get active technik by firm ID within a time range.
        // ----------------------------------------
        public async Task<string> GetTechnikActivTechnikByIdFirmaAsync(string idFirma, DateTime OD, DateTime DO)
        {
            var technik = await _pohotovostiRepository.GetTechnikVPohotovostiAsnyc(idFirma, OD, DO);
            return technik;
        }
    }
}