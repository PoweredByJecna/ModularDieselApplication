using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<TableUser, IdentityRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // ----------------------------------------
            // Configure default schema for Identity tables.
            // ----------------------------------------
            builder.HasDefaultSchema("Identity");
            base.OnModelCreating(builder);

            // ----------------------------------------
            // Configure Identity tables.
            // ----------------------------------------
            builder.Entity<TableUser>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // ----------------------------------------
            // Configure application-specific tables with schema "Data".
            // ----------------------------------------
            builder.Entity<TableLokality>().ToTable("LokalityTable", schema: "Data");
            builder.Entity<TableOdstavky>().ToTable("OdstavkyTable", schema: "Data");
            builder.Entity<TableDieslovani>().ToTable("TableDieslovani", schema: "Data");
            builder.Entity<TableFirma>().ToTable("TableFirma", schema: "Data");
            builder.Entity<TableRegiony>().ToTable("TableRegiony", schema: "Data");
            builder.Entity<TablePohotovosti>().ToTable("TablePohotovosti", schema: "Data");
            builder.Entity<TableTechnici>().ToTable("TableTechnici", schema: "Data");
            builder.Entity<DebugLogModel>().ToTable("DebugModel", schema: "Data");
            builder.Entity<TableZdroj>().ToTable("TableZdroj", schema: "Data");

            // ----------------------------------------
            // Configure relationships for TableOdstavky.
            // ----------------------------------------
            builder.Entity<TableOdstavky>()
                .HasOne(o => o.Lokality) // One Lokality can have many Odstavky.
                .WithMany(l => l.OdstavkyList) // Navigation property in Lokality.
                .HasForeignKey(o => o.LokalitaID) // Foreign key in TableOdstavky.
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior.

            // ----------------------------------------
            // Configure relationships for TableDieslovani.
            // ----------------------------------------
            builder.Entity<TableDieslovani>()
                .HasOne(o => o.Odstavka) // One Odstavka can have many Dieslovani.
                .WithMany(i => i.DieslovaniList) // Navigation property in Odstavka.
                .HasForeignKey(o => o.IDodstavky) // Foreign key in TableDieslovani.
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior.

            builder.Entity<TableDieslovani>()
                .HasOne(o => o.Technik) // One Technik can have many Dieslovani.
                .WithMany(i => i.DieslovaniList) // Navigation property in Technik.
                .HasForeignKey(o => o.IdTechnik); // Foreign key in TableDieslovani.
        }

        // ----------------------------------------
        // DbSet properties for application tables.
        // ----------------------------------------
        public DbSet<TableLokality> LokalityS { get; set; }
        public DbSet<TableOdstavky> OdstavkyS { get; set; }
        public DbSet<TableDieslovani> DieslovaniS { get; set; }
        public DbSet<TableFirma> FirmaS { get; set; }
        public DbSet<TableRegiony> RegionS { get; set; }
        public DbSet<TablePohotovosti> PohotovostiS { get; set; }
        public DbSet<TableTechnici> TechnikS { get; set; }
        public DbSet<DebugLogModel> LogS { get; set; }
    }
}
