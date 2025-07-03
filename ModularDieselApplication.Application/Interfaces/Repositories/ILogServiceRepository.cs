using System;
using System.Collections.Generic;
using ModularDieselApplication.Domain.Entities;


namespace ModularDieselApplication.Application.Interfaces.Repositories
{
    public interface ILogServiceRepository
    {
        Task<IEnumerable<Log>> GetLogByEntityAsync(string id);
        Task AddLogAsync(Log logEntry);
    }
}