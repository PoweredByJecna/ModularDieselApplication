using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces.Repositories;
using static ModularDieselApplication.Application.Services.OdstavkyService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniAssignmentService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniActionService;
using ModularDieselApplication.Application.Services.DieslovaniServices.DieslovaniQueryService;
using ModularDieselApplication.Domain.Objects;



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
        /* ----------------------------------------
           GetTableDataAllTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataAllTableAsync(User? currentUser, bool isEngineer)
        {
            return await _dieslovaniQueryService.GetTableDataAllTableAsync(currentUser, isEngineer);
        }
        /* ----------------------------------------
           GetTableDataRunningTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataRunningTableAsync(User? currentUser, bool isEngineer)
        {
            return await _dieslovaniQueryService.GetTableDataRunningTableAsync(currentUser, isEngineer);
        }
        /* ----------------------------------------
           GetTableDataUpcomingTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataUpcomingTableAsync(User? currentUser, bool isEngineer)
        {
            return await _dieslovaniQueryService.GetTableDataUpcomingTableAsync(currentUser, isEngineer);
        }
        /* ----------------------------------------
           GetTableDataEndTableAsync
           ---------------------------------------- */
        public async Task<(int totalRecords, List<object> data)> GetTableDataEndTableAsync(User? currentUser, bool isEngineer)
        {
            return await _dieslovaniQueryService.GetTableDataEndTableAsync(currentUser, isEngineer);
        }
        /* ----------------------------------------
           GetTableDatathrashTableAsync
           ---------------------------------------- */
        public async Task<List<object>> GetTableDatathrashTableAsync(User? currentUser, bool isEngineer)
        {
            return await _dieslovaniQueryService.GetTableDatathrashTableAsync(currentUser, isEngineer);
        }
        /* ----------------------------------------
           GetTableDataOdDetailOdstavkyAsync
           ---------------------------------------- */
        public async Task<List<object>> GetTableDataOdDetailOdstavkyAsync(int idodstavky)
        {
            return await _dieslovaniQueryService.GetTableDataOdDetailOdstavkyAsync(idodstavky);
        }
        /* ----------------------------------------
           HandleOdstavkyDieslovani
           ---------------------------------------- */
        public async Task<HandleResult> HandleOdstavkyDieslovani(Odstavka? newOdstavka, HandleResult result)
        {
            await _dieslovaniAssignmentService.HandleOdstavkyDieslovani(newOdstavka, result);
            return result;
        }
        /* ----------------------------------------
           VstupAsync
        ---------------------------------------- */
        public async Task<HandleResult> VstupAsync(int idDieslovani)
        {
            return await _dieslovaniActionService.VstupAsync(idDieslovani);
        }
        /* ----------------------------------------
           OdchodAsync
           ---------------------------------------- */
        public async Task<HandleResult> OdchodAsync(int idDieslovani)
        {
            return await _dieslovaniActionService.OdchodAsync(idDieslovani);
        }
        /* ----------------------------------------
           TemporaryLeaveAsync
           ---------------------------------------- */
        public async Task<(bool Success, string Message)> TemporaryLeaveAsync(int idDieslovani)
        {
            return await _dieslovaniActionService.TemporaryLeaveAsync(idDieslovani);
        }
        /* ----------------------------------------
           TakeAsync
           ---------------------------------------- */
        public async Task<HandleResult> TakeAsync(int idDieslovani, User currentUser)
        {
            return await _dieslovaniActionService.TakeAsync(idDieslovani, currentUser);
        }
        /* ----------------------------------------
           DetailDieslovaniAsync
           ---------------------------------------- */
        public async Task<Dieslovani?> DetailDieslovaniAsync(int id)
        {
            return await _dieslovaniRepository.GetByIdAsync(id);
        }
        /* ----------------------------------------
           DetailDieslovaniJsonAsync
           ---------------------------------------- */
        public async Task<object> DetailDieslovaniJsonAsync(int id)
        {
            return await _dieslovaniQueryService.DetailDieslovaniJsonAsync(id);
        }
        /* ----------------------------------------
           DeleteDieslovaniJsonAsync
           ---------------------------------------- */
        public async Task<(bool Success, string Message)> DeleteDieslovaniAsync(int idDieslovani)
        {
           return await _dieslovaniActionService.DeleteDieslovaniAsync(idDieslovani);
        }
        /* ----------------------------------------
           GetDieslovaniByOdstavkaId
        ---------------------------------------- */
        public async Task<Dieslovani> GetDieslovaniByOdstavkaId(int id)
        {
            return await _dieslovaniRepository.GetDAbyOdstavkaAsync(id);
        }
        public async Task<int> GetOdstavkaIDbyDieselId(int idDieslovani)
        {
            return await _dieslovaniRepository.GetIDbyDieselId(idDieslovani);
        }

      
    }    
}

       