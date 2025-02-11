
using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Repositories;

namespace ModularDieselApplication.Application.Services
{
    
    public class LogService : ILogService
    {
        private readonly ILogServiceRepository _Ilogservice;

        public LogService(ILogServiceRepository logService)
        {
            _Ilogservice = logService ?? throw new ArgumentNullException(nameof(logService));
        }

        public async Task<object> GetLogByEntityAsync(int id)
        {
            var log = await _Ilogservice.GetLogByEntityAsync(id);
            return log;
        }
        public async Task LogAsync(Log logEntry)
        {
            if (logEntry.TimeStamp == default)
                logEntry.TimeStamp = DateTime.Now;
            await _Ilogservice.AddLogAsync(logEntry);
    
        }
        public async Task<Log>ZapisDoLogu(DateTime datum, string entityname, int entityId, string logmessage)
        {
              var logEntry = new Log
                {
                    TimeStamp = datum,
                    EntityName = entityname,
                    EntityId = entityId,
                    LogMessage = logmessage 
                };
                await LogAsync(logEntry);

            return logEntry;    

        }

    }
}