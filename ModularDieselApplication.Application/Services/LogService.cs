using ModularDieselApplication.Application.Interfaces.Services;
using ModularDieselApplication.Domain.Entities;
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

        // ----------------------------------------
        // Retrieve logs for a specific entity by ID.
        // ----------------------------------------
        public async Task<object> GetLogByEntityAsync(string id)
        {
            var log = await _Ilogservice.GetLogByEntityAsync(id);
            return log;
        }

        // ----------------------------------------
        // Add a log entry to the database.
        // ----------------------------------------
        public async Task LogAsync(Log logEntry)
        {
            if (logEntry.TimeStamp == default)
                logEntry.TimeStamp = DateTime.Now;

            await _Ilogservice.AddLogAsync(logEntry);
        }

        // ----------------------------------------
        // Create and save a log entry with specific details.
        // ----------------------------------------
        public async Task<Log> ZapisDoLogu(DateTime datum, string entityname, string entityId, string logmessage)
        {
            var logEntry = new Log
            {
                IdLog = Guid.NewGuid().ToString(),
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