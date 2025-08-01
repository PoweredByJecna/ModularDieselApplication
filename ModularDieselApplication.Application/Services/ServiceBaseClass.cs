using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Domain.Objects;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Services;

public class ServiceBaseClass : IServiceBaseClass
{
    private readonly IDieslovaniRepository _dieslovaniRepository;
    private readonly IOdstavkyRepository _odstavkaRepository;
    private readonly IDieslovaniService _dieslovaniService;
    private readonly IOdstavkyService _odstavkaService;
    public ServiceBaseClass(IDieslovaniRepository dieslovaniRepository, IDieslovaniService dieslovaniService, IOdstavkyRepository odstavkaRepository, IOdstavkyService odstavkaService)
    {
        _dieslovaniRepository = dieslovaniRepository;
        _dieslovaniService = dieslovaniService;
        _odstavkaRepository = odstavkaRepository;
        _odstavkaService = odstavkaService;
    }
    public async Task<List<Object>> GetTableData(ServiceFilterEnum serviceFilter, DieslovaniOdstavkaFilterEnum filter, User currentUser =null, bool isEngineer =default)
    {
        switch (serviceFilter)
        {
            case ServiceFilterEnum.Dieslovani:
                switch (filter)
                {
                    case DieslovaniOdstavkaFilterEnum.AllTable:
                        return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Cast<object>().ToListAsync();
                    case DieslovaniOdstavkaFilterEnum.RunningTable:
                        return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup != DateTime.MinValue && i.Odchod == DateTime.MinValue).Cast<object>().ToListAsync();
                    case DieslovaniOdstavkaFilterEnum.UpcomingTable:
                        return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID != FiktivniTechnik.Id).Cast<object>().ToListAsync();
                    case DieslovaniOdstavkaFilterEnum.EndTable:
                        return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Odchod != DateTime.MinValue.Date && i.Odstavka.Do.Date <= DateTime.Today).Cast<object>().ToListAsync();
                    case DieslovaniOdstavkaFilterEnum.TrashTable:
                        return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID == FiktivniTechnik.Id).Cast<object>().ToListAsync();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
                }
            case ServiceFilterEnum.Odstavka:
                switch (filter)
                {
                    case DieslovaniOdstavkaFilterEnum.OD:
                        return await _odstavkaRepository.GetOdstavkaQuery().Cast<object>().ToListAsync();
                    default:
                        throw new ArgumentOutOfRangeException(nameof(serviceFilter), serviceFilter, null);
                }
            default:
                throw new ArgumentOutOfRangeException(nameof(serviceFilter), serviceFilter, null);
        }
    }
    public async Task<HandleResult> ActionMethods(ServiceFilterEnum serviceFilter, ActionFilter filter, string Id, DateTime time = default, User? currentUser = null)
    {
        try
        {
            switch (serviceFilter)
            {
                case ServiceFilterEnum.Dieslovani:
                    switch (filter)
                    {
                        case ActionFilter.Vstup:
                            await _dieslovaniService.VstupAsync(Id);
                            return HandleResult.OK("Vstup byl úspěšně zaznamenán.");
                        case ActionFilter.Odchod:
                            await _dieslovaniService.OdchodAsync(Id);
                            return HandleResult.OK("Odchod byl úspěšně zaznamenán.");
                        case ActionFilter.Delete:
                            await _dieslovaniService.DeleteDieslovaniAsync(Id);
                            return HandleResult.OK("Dieslovani byla úspěšně smazána.");
                        case ActionFilter.CallDA:
                            await _dieslovaniService.CallDieslovaniAsync(Id);
                            return HandleResult.OK("Dieslovani bylo úspěšně objednáno.");
                        case ActionFilter.ChangeTimeVstup:
                            await _dieslovaniService.ChangeTimeAsync(Id, time, ActionFilter.Vstup);
                            return HandleResult.OK("Čas byl úspěšně změněn.");
                        case ActionFilter.ChangeTimeOdchod:
                            await _dieslovaniService.ChangeTimeAsync(Id, time, ActionFilter.Odchod);
                            return HandleResult.OK("Čas byl úspěšně změněn.");
                        case ActionFilter.take:
                            await _dieslovaniService.TakeAsync(Id, currentUser);
                            return HandleResult.OK("Dieslovani bylo úspěšně převzato.");

                        default:
                            throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
                    }
                case ServiceFilterEnum.Odstavka:
                    switch (filter)
                    {
                        case ActionFilter.ChangeTimeZactek:
                            await _odstavkaService.ChangeTimeOdstavkyAsync(Id, time, ActionFilter.zacatek);
                            return HandleResult.OK("Čas byl úspěšně změněn.");
                        case ActionFilter.ChangeTimeKonec:
                            await _odstavkaService.ChangeTimeOdstavkyAsync(Id, time, ActionFilter.zacatek);
                            return HandleResult.OK("Čas byl úspěšně změněn.");
                        case ActionFilter.Delete:
                            await _odstavkaService.DeleteOdstavkaAsync(Id);
                            return HandleResult.OK("Uspěšně smazano");
                        default:
                            throw new NotImplementedException("Odstavky action filter is not implemented yet.");
                    }
                default:
                    throw new ArgumentOutOfRangeException(nameof(serviceFilter), serviceFilter, null);
            }
        }

        catch (Exception ex)
        {
            return HandleResult.Error($"Chyba při zpracování akce na {serviceFilter}: {ex.Message}");
        }
    }
}