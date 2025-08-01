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
            bool isEngineer = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Engineer");
            bool isAdmin = currentUser != null && await _userService.IsUserInRoleAsync(currentUser.Id, "Admin");

            if (!isEngineer && !isAdmin)
            {

                return HandleResult.Error("Nemáš oprávnění");
            }

            if (zacatek.Date > konec.Date)
            {
                return HandleResult.Error("Neplatný interval pohotvosti");
            }

            if (isEngineer)
            {
                await pohotovost(currentUser, zacatek, konec);
            }
            if (isAdmin)
            {
                await pohotovost(currentUser, zacatek, konec);
            }

            return HandleResult.Error("Nepodařilo se provést zápis pohotovosti");
         
        }
        private async Task<HandleResult> pohotovost(User currentUser,DateTime zacatek, DateTime konec)
        {
        if (currentUser == null)
            {
                return HandleResult.Error("Neplatný uživatel");
            }

            var technikSearch = await _technikRepository.GetTechnik(Domain.Enum.GetTechnikEnum.ByUserId, currentUser.Id);

            if (technikSearch == null)
            {
                return HandleResult.Error("Nepodařilo se najít technika přiřazeného k aktuálnímu uživateli");
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

            return HandleResult.OK("Pohotovost byla zapsána");
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
        public async Task<Technik> GetTechnikAsync(string idFirma, DateTime OD, DateTime DO)
        {
            return await _pohotovostiRepository.GetTechnikVPohotovostiAsnyc(idFirma, OD, DO);
        }
    }
}