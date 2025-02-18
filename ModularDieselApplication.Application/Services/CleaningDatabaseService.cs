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

        // do konstruktoru si necháme injektovat pouze IServiceScopeFactory
        public CleaningDatabaseService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // první volání okamžitě (TimeSpan.Zero) a pak opakování 1× za den
            _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromDays(1));
            return Task.CompletedTask;
        }

        private void DoWork(object? state)
        {
            // Tady si vytvoříme nový scope
            using var scope = _scopeFactory.CreateScope();

            // Z toho scope si vytáhneme IDatabaseCleaner
            var databaseCleaner = scope.ServiceProvider.GetRequiredService<IDatabaseCleaner>();

            // Zavoláme async metodu - je potřeba nějak vyřešit await.
            // Pro ukázku "synchronně" počkáme (nebo použijte jiný pattern, např. async void / Task.Run atd.)
            databaseCleaner.CleanOutdatedRecords().GetAwaiter().GetResult();
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}