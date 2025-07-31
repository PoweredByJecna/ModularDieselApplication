using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniActionService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniQueryService;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Metadata;


namespace ModularDieselApplication.Application.Services
{
    public class DieslovaniService : IDieslovaniService
    {
        private readonly IDieslovaniRepository _dieslovaniRepository;
        private readonly DieslovaniQueryService _dieslovaniQueryService;
        private readonly DieslovaniActionService _dieslovaniActionService;
        private readonly DieslovaniAssignmentService _dieslovaniAssignmentService;

        public DieslovaniService(
            IDieslovaniRepository dieslovaniRepository,
            DieslovaniQueryService dieslovaniQueryService,
            DieslovaniActionService dieslovaniActionService,
            DieslovaniAssignmentService dieslovaniAssignmentService
        )
        {
            _dieslovaniRepository = dieslovaniRepository;
            _dieslovaniQueryService = dieslovaniQueryService;
            _dieslovaniActionService = dieslovaniActionService;
            _dieslovaniAssignmentService = dieslovaniAssignmentService;
        }
        // ----------------------------------------
        // Handle dieslovani for an odstávka.
        // ----------------------------------------
        public async Task<HandleResult<Dieslovani>> HandleOdstavkyDieslovani(Odstavka? newOdstavka)
        {
            var result = await _dieslovaniAssignmentService.HandleOdstavkyDieslovani(newOdstavka);
            return result;
        }
        // ----------------------------------------
        // Get dieslovani records by user ID.
        // ----------------------------------------
        public async Task<HandleResult> ActioOnDieslovani(ActionFilter filter, string Id, DateTime time  = default)
        {
            try
            {
                switch (filter)
                {
                    case ActionFilter.Vstup:
                        await _dieslovaniActionService.VstupAsync(Id);
                        return HandleResult.OK("Vstup byl úspěšně zaznamenán.");
                    case ActionFilter.Odchod:
                        await _dieslovaniActionService.OdchodAsync(Id);
                        return HandleResult.OK("Odchod byl úspěšně zaznamenán.");
                    case ActionFilter.Delete:
                        await _dieslovaniActionService.DeleteDieslovaniAsync(Id);
                        return HandleResult.OK("Dieslovani byla úspěšně smazána.");
                    case ActionFilter.CallDA:
                        await _dieslovaniAssignmentService.CallDieslovaniAsync(Id);
                        return HandleResult.OK("Dieslovani bylo úspěšně objednáno.");
                    case ActionFilter.ChangeTime:
                        await _dieslovaniActionService.ChangeTimeAsync(Id, time, ActionFilter.Vstup);
                        return HandleResult.OK("Čas byl úspěšně změněn.");
                    default:
                        throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
                }
            }
            catch (Exception ex)
            {
                return HandleResult.Error($"Chyba při zpracování akce na dieslovani: {ex.Message}");
            }
        }
        // ----------------------------------------
        // Check if another dieslovani request exists.
        // ----------------------------------------
        public async Task<bool> AnotherDieselRequestAsync(string idTechnika)
        {
            return await _dieslovaniRepository.AnotherDieselRequest(idTechnika);
        }
        public async Task<List<Dieslovani>> GetTableData(DieslovaniFilterEnum filter, User currentUser, bool isEngineer)
        {
            switch (filter)
            {
                case DieslovaniFilterEnum.AllTable:
                    return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).ToListAsync();
                case DieslovaniFilterEnum.RunningTable:
                    return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup != DateTime.MinValue && i.Odchod == DateTime.MinValue).ToListAsync();
                case DieslovaniFilterEnum.UpcomingTable:
                    return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID != FiktivniTechnik.Id).ToListAsync();
                case DieslovaniFilterEnum.EndTable:
                    return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Odchod != DateTime.MinValue.Date && i.Odstavka.Do.Date <= DateTime.Today).ToListAsync();
                case DieslovaniFilterEnum.TrashTable:
                    return await _dieslovaniRepository.GetDieslovaniQuery(currentUser, isEngineer).Where(i => i.Vstup == DateTime.MinValue.Date && i.Odstavka.Od.Date == DateTime.Today && i.Technik.ID == FiktivniTechnik.Id).ToListAsync();
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }
    }
}

