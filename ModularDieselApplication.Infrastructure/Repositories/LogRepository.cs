using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Persistence.Repositories
{
    public class LogRepository : ILogServiceRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // ----------------------------------------
        // Get logs by entity ID.
        // ----------------------------------------
        public async Task<IEnumerable<Log>> GetLogByEntityAsync(string id)
        {
            // Retrieve logs from the database where the EntityId matches the given ID.
            var logModels = await _context.Set<DebugLogModel>()
                .Where(l => l.EntityId == id)
                .ToListAsync();

            // Map the database models to domain entities and return them.
            return logModels.Select(l => new Log
            {
                IdLog = l.IdLog,
                TimeStamp = l.TimeStamp,
                EntityName = l.EntityName,
                EntityId = l.EntityId,
                LogMessage = l.LogMessage
            }).ToList();
        }

        // ----------------------------------------
        // Add a new log entry.
        // ----------------------------------------
        public async Task AddLogAsync(Log logEntry)
        {
            // Map the domain entity to a database model.
            var logModel = new DebugLogModel
            {
                IdLog = logEntry.IdLog,
                TimeStamp = logEntry.TimeStamp,
                EntityName = logEntry.EntityName,
                EntityId = logEntry.EntityId,
                LogMessage = logEntry.LogMessage
            };

            // Add the log entry to the database and save changes.
            await _context.Set<DebugLogModel>().AddAsync(logModel);
            await _context.SaveChangesAsync();
        }
    }
}