using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Application.Interfaces;
namespace ModularDieselApplication.Infrastructure.CleaningDatabase
{
    public class DatabaseCleaner : IDatabaseCleaner
    {
        private readonly IServiceScopeFactory _scopeFactory;
        public DatabaseCleaner(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }
        public async Task CleanOutdatedRecords()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var outdatedRecordsOdstavky = await _context.OdstavkyS
                    .Where(d => d.Do.Date < DateTime.Today).ToListAsync();

                var outdatedRecordsDieslovani = await _context.DieslovaniS.Include(d => d.Technik)
                    .Where(d => d.Odstavka.Do.Date < DateTime.Today).ToListAsync();

                var outdatedRecordPohotovosti = await _context.PohotovostiS
                    .Where(d => d.Konec.Date < DateTime.Today).ToListAsync();

                if (outdatedRecordsOdstavky.Any())
                {
                    _context.OdstavkyS.RemoveRange(outdatedRecordsOdstavky);
                    await _context.SaveChangesAsync();
                }
                foreach (var dieslovani in outdatedRecordsDieslovani)
                {
                    if (dieslovani.Technik != null)
                    {
                        dieslovani.Technik.Taken = false;
                        _context.TechnikS.Update(dieslovani.Technik);
                        await _context.SaveChangesAsync();
                    }
                }
                if (outdatedRecordPohotovosti.Any())
                {
                    _context.PohotovostiS.RemoveRange(outdatedRecordPohotovosti);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}