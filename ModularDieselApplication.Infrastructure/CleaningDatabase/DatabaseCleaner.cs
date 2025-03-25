using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ModularDieselApplication.Infrastructure.Persistence;
using ModularDieselApplication.Application.Interfaces;
using ModularDieselApplication.Application.Interfaces.Repositories;
using ModularDieselApplication.Domain.Entities;
using ModularDieselApplication.Interfaces.Repositories;

namespace ModularDieselApplication.Infrastructure.CleaningDatabase
{
    public class DatabaseCleaner : IDatabaseCleaner
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IDieslovaniRepository _dieslovaniRepository;
        private readonly IOdstavkyRepository _odstavkyRepository;
        private readonly IPohotovostiRepository _pohotovostiRepository;
        private readonly ITechniciRepository _techniciRepository;

        public DatabaseCleaner(IServiceScopeFactory scopeFactory, IDieslovaniRepository dieslovaniRepository, IOdstavkyRepository odstavkyRepository, IPohotovostiRepository pohotovostiRepository, ITechniciRepository techniciRepository)
        {
            _scopeFactory = scopeFactory;
            _dieslovaniRepository = dieslovaniRepository;
            _odstavkyRepository = odstavkyRepository;
            _pohotovostiRepository = pohotovostiRepository;
            _techniciRepository = techniciRepository;
        }

        // ----------------------------------------
        // Clean outdated records from the database.
        // ----------------------------------------
        public async Task CleanOutdatedRecords()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                // Retrieve outdated odstávky records.
                var outdatedRecordsOdstavky = await _context.OdstavkyS
                    .Where(d => d.Do.Date.AddDays(90) < DateTime.Today).ToListAsync();

                // Retrieve outdated dieslovani records.
                var outdatedRecordsDieslovani = await _context.DieslovaniS.Include(d => d.Technik)
                    .Where(d => d.Odstavka.Do.Date.AddDays(90) < DateTime.Today).ToListAsync();

                // Retrieve outdated pohotovosti records.
                var outdatedRecordPohotovosti = await _context.PohotovostiS
                    .Where(d => d.Konec.Date < DateTime.Today).ToListAsync();

                // Process outdated dieslovani records.
                foreach (var dieslovani in outdatedRecordsDieslovani)
                {
                    var technik = dieslovani.Technik;
                    await _dieslovaniRepository.DeleteAsync(dieslovani.ID);
                    await _context.SaveChangesAsync();

                    if (technik != null)
                    {
                        var anotherDieselRequest = await _dieslovaniRepository.AnotherDieselRequest(technik.ID);
                        if (!anotherDieselRequest)
                        {
                            dieslovani.Technik.Taken = false;
                            _context.TechnikS.Update(technik);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                // Process outdated odstávky records.
                if (outdatedRecordsOdstavky.Any())
                {
                    foreach (var odstavka in outdatedRecordsOdstavky)
                    {
                        var odstavkaDieslovani = await _dieslovaniRepository.GetDAbyOdstavkaAsync(odstavka.ID);
                        if (odstavkaDieslovani != null)
                        {
                            var technikId = odstavkaDieslovani.Technik.ID;
                            await _odstavkyRepository.DeleteAsync(odstavka.ID);
                            await _context.SaveChangesAsync();

                            if (odstavkaDieslovani != null)
                            {
                                var anotherDA = await _dieslovaniRepository.AnotherDieselRequest(technikId);
                                var technik = await _techniciRepository.GetByIdAsync(technikId);
                                if (!anotherDA)
                                {
                                    technik.Taken = false;
                                    await _techniciRepository.UpdateAsync(technik);
                                    await _context.SaveChangesAsync();
                                }
                            }
                        }
                        else
                        {
                            await _odstavkyRepository.DeleteAsync(odstavka.ID);
                            await _context.SaveChangesAsync();
                        }
                    }
                }

                // Remove outdated pohotovosti records.
                if (outdatedRecordPohotovosti.Any())
                {
                    _context.PohotovostiS.RemoveRange(outdatedRecordPohotovosti);
                    await _context.SaveChangesAsync();
                }
            }
        }
    }
}