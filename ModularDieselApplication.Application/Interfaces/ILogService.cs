using System;
using System.Threading.Tasks;
using ModularDieselApplication.Domain.Entities;


namespace ModularDieselApplication.Application.Services
{
    public interface ILogService
    {
        Task LogAsync(Log logEntry);
        Task<object> GetLogByEntityAsync(string id);
        Task<Log> ZapisDoLogu(DateTime datum, string entityname, string entityId, string logmessage);
    }
}