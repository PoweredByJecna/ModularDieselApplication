using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ModularDieselApplication.Infrastructure.Persistence
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        // ----------------------------------------
        // Create a new instance of ApplicationDbContext for design-time operations.
        // ----------------------------------------
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Build the configuration from the appsettings.Development.json file.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ModularDieselApplication.Api"))
                .AddJsonFile("appsettings.Development.json", optional: false)
                .Build();

            // Configure the DbContext options to use the SQL Server connection string.
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

            // Return a new instance of ApplicationDbContext with the configured options.
            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
