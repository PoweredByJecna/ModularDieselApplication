using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniActionService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniQueryService;
using ModularDieselApplication.Domain.Objects;
using ModularDieselApplication.Domain.Enum;

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
        // Get dieslovani detail data as JSON.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataDetailJsonAsync(string id)
        {
            return await _dieslovaniQueryService.GetTableDataDetailJsonAsync(id);
        }
        // ----------------------------------------
        // Get dieslovani details for a specific odstávka.
        // ----------------------------------------
        public async Task<List<object>> GetTableDataOdDetailOdstavkyAsync(string idodstavky)
        {
            return await _dieslovaniQueryService.GetTableDataOdDetailOdstavkyAsync(idodstavky);
        }

        // ----------------------------------------
        // Handle dieslovani for an odstávka.
        // ----------------------------------------
        public async Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result)
        {
            await _dieslovaniAssignmentService.HandleOdstavkyDieslovani(newOdstavka, result);
            return result;
        }

        // ----------------------------------------
        // Record entry to a location.
        // ----------------------------------------
        public async Task<HandleResult> VstupAsync(string idDieslovani)
        {
            return await _dieslovaniActionService.VstupAsync(idDieslovani);
        }

        // ----------------------------------------
        // Record exit from a location.
        // ----------------------------------------
        public async Task<HandleResult> OdchodAsync(string idDieslovani)
        {
            return await _dieslovaniActionService.OdchodAsync(idDieslovani);
        }

        // ----------------------------------------
        // Toggle temporary leave status.
        // ----------------------------------------
        public async Task<(bool Success, string Message)> TemporaryLeaveAsync(string idDieslovani)
        {
            return await _dieslovaniActionService.TemporaryLeaveAsync(idDieslovani);
        }

        // ----------------------------------------
        // Assign a technician to a location.
        // ----------------------------------------
        public async Task<HandleResult> TakeAsync(string idDieslovani, User currentUser)
        {
            return await _dieslovaniActionService.TakeAsync(idDieslovani, currentUser);
        }

        // ----------------------------------------
        // Get dieslovani details by ID.
        // ----------------------------------------
        public async Task<Dieslovani?> DetailDieslovaniAsync(string id)
        {
            return await _dieslovaniRepository.GetByIdAsync(id);
        }

        // ----------------------------------------
        // Get dieslovani details as JSON.
        // ----------------------------------------
        public async Task<object> DetailDieslovaniJsonAsync(string id)
        {
            return await _dieslovaniQueryService.DetailDieslovaniJsonAsync(id);
        }

        // ----------------------------------------
        // Delete a dieslovani record.
        // ----------------------------------------
        public async Task<HandleResult> DeleteDieslovaniAsync(string idDieslovani)
        {
            return await _dieslovaniActionService.DeleteDieslovaniAsync(idDieslovani);
        }

        // ----------------------------------------
        // Get dieslovani by odstávka ID.
        // ----------------------------------------
        public async Task<Dieslovani> GetDieslovaniByOdstavkaId(string id)
        {
            return await _dieslovaniRepository.GetDAbyOdstavkaAsync(id);
        }

        // ----------------------------------------
        // Get odstávka ID by dieslovani ID.
        // ----------------------------------------
        public async Task<string> GetOdstavkaIDbyDieselId(string idDieslovani)
        {
            return await _dieslovaniRepository.GetIDbyDieselId(idDieslovani);
        }

        // ----------------------------------------
        // Get dieslovani records by user ID.
        // ----------------------------------------
        public async Task<List<object>> GetDieslovaniByUserId(string idUser)
        {
            return await _dieslovaniQueryService.GetDieslovaniByUserId(idUser);
        }

        // ----------------------------------------
        // Check if another dieslovani request exists.
        // ----------------------------------------
        public async Task<bool> AnotherDieselRequestAsync(string idTechnika)
        {
            return await _dieslovaniRepository.AnotherDieselRequest(idTechnika);
        }

        // ----------------------------------------
        // Change the time for a dieslovani record.
        // ----------------------------------------
        public async Task<HandleResult> ChangeTimeAsync(string idDieslovani, DateTime time, string type)
        {
            return await _dieslovaniActionService.ChangeTimeAsync(idDieslovani, time, type);
        }

        // ----------------------------------------
        // Call dieslovani for an odstávka.
        // ----------------------------------------
        public async Task<HandleResult> CallDieslovaniAsync(string odstavky)
        {
            return await _dieslovaniAssignmentService.CallDieslovaniAsync(odstavky);
        }

        public async Task<List<object>> GetTableData(DieslovaniFilterEnum filter, User currentUser, bool isEngineer)
        {
            switch (filter)
            {
                case DieslovaniFilterEnum.AllTable:
                    return await _dieslovaniQueryService.GetTableDataAllTableAsync(currentUser, isEngineer);
                case DieslovaniFilterEnum.RunningTable:
                    return await _dieslovaniQueryService.GetTableDataRunningTableAsync(currentUser, isEngineer);
                case DieslovaniFilterEnum.UpcomingTable:
                    return await _dieslovaniQueryService.GetTableDataUpcomingTableAsync(currentUser, isEngineer);
                case DieslovaniFilterEnum.EndTable:
                    return await _dieslovaniQueryService.GetTableDataEndTableAsync(currentUser, isEngineer);
                case DieslovaniFilterEnum.TrashTable:
                    return await _dieslovaniQueryService.GetTableDatathrashTableAsync(currentUser, isEngineer);
                default:
                    throw new ArgumentOutOfRangeException(nameof(filter), filter, null);
            }
        }
    }
}

