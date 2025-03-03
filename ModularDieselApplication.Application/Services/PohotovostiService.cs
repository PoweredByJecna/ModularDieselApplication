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

        public async Task<List<Pohotovosti>> GetAllPohotovostiAsync()
        {
            return await _pohotovostiRepository.GetAllPohotovostiAsync();
        }
        public async Task<bool> PohovostiVRegionuAsync(int idRegionu)
        {
            return await _pohotovostiRepository.GetPohotovostiRegionAsync(idRegionu);
        }

        public async Task<HandleResult> ZapisPohotovostAsync(Pohotovosti pohotovosti, User currentUser)
        {
            var result  =  new HandleResult();
            bool isEngineer = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Engineer");
            bool isAdmin = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Admin");

            if (!isEngineer && !isAdmin)
            {
                result.Success = false;
                result.Message = "Nemáte oprávnění zapsat pohotovost.";
                return result;
            }

            if (pohotovosti.Zacatek <= pohotovosti.Zacatek || pohotovosti.Zacatek < DateTime.Today)
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
                    IdUser = technikSearch.User.Id,
                    User = technikSearch.User,
                    Zacatek = pohotovosti.Zacatek,
                    Konec = pohotovosti.Konec,
                    IdTechnik = technikSearch.ID
                };

                await _pohotovostiRepository.AddPohotovostAsync(zapis);

                result.Success = true;
                result.Message = "Pohotovost byla úspěšně zapsána.";
                return result;
            }

            if (isAdmin)
            {
                var technikSearch = await _pohotovostiRepository.GetPohotovostTechnikIdsAsync(pohotovosti.Technik.ID);

                if (technikSearch == null)
                {
                    result.Success = false;
                    result.Message = "Nepodařilo se najít technika podle zadaného IdTechnika.";
                    return result;
                }

                var zapis = new Pohotovosti
                {
                    IdUser = technikSearch.User.Id,
                    Zacatek = pohotovosti.Zacatek,
                    Konec = pohotovosti.Konec,
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
        public async Task<string> GetTechnikActivTechnikByIdFirmaAsync(int idFirma)
        {
            var technik = await _pohotovostiRepository.GetTechnikVPohotovostiAsnyc(idFirma);
            return technik;
        }
        

    }
}