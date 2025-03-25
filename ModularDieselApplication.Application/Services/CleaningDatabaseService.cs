using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ModularDieselApplication.Application.Interfaces;

namespace ModularDieselApplication.Application.Services
{
    public class CleaningDatabaseService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private Timer? _timer;

        public CleaningDatabaseService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        // ----------------------------------------
        // Start the background service.
        // ----------------------------------------
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Schedule the DoWork method to run daily.
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        // ----------------------------------------
        // Perform the database cleaning task.
        // ----------------------------------------
        private void DoWork(object? state)
        {
            using var scope = _scopeFactory.CreateScope();
            var databaseCleaner = scope.ServiceProvider.GetRequiredService<IDatabaseCleaner>();
            databaseCleaner.CleanOutdatedRecords().GetAwaiter().GetResult();
        }

        // ----------------------------------------
        // Stop the background service.
        // ----------------------------------------
        public Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop the timer to halt scheduled tasks.
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        // ----------------------------------------
        // Dispose of resources used by the service.
        // ----------------------------------------
        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}