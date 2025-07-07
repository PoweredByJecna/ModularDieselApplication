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
            builder.Entity<TableLokalita>().ToTable("TableLokalita", schema: "Data");
            builder.Entity<TableOdstavka>().ToTable("TableOdstavka", schema: "Data");
            builder.Entity<TableDieslovani>().ToTable("TableDieslovani", schema: "Data");
            builder.Entity<TableFirma>().ToTable("TableFirma", schema: "Data");
            builder.Entity<TableRegion>().ToTable("TableRegion", schema: "Data");
            builder.Entity<TablePohotovost>().ToTable("TablePohotovost", schema: "Data");
            builder.Entity<TableTechnik>().ToTable("TableTechnik", schema: "Data");
            builder.Entity<DebugLogModel>().ToTable("DebugModel", schema: "Data");
            builder.Entity<TableZdroj>().ToTable("TableZdroj", schema: "Data");

            // ----------------------------------------
            // Configure relationships for TableOdstavky.
            // ----------------------------------------
            builder.Entity<TableOdstavka>()
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
                .HasForeignKey(o => o.IdOdstavky) // Foreign key in TableDieslovani.
                .OnDelete(DeleteBehavior.Cascade); // Cascade delete behavior.

            builder.Entity<TableDieslovani>()
                .HasOne(o => o.Technik) // One Technik can have many Dieslovani.
                .WithMany(i => i.DieslovaniList) // Navigation property in Technik.
                .HasForeignKey(o => o.IdTechnik); // Foreign key in TableDieslovani.
        }

        // ----------------------------------------
        // DbSet properties for application tables.
        // ----------------------------------------
        public DbSet<TableLokalita> LokalityS { get; set; }
        public DbSet<TableOdstavka> OdstavkyS { get; set; }
        public DbSet<TableDieslovani> DieslovaniS { get; set; }
        public DbSet<TableFirma> FirmaS { get; set; }
        public DbSet<TableRegion> RegionS { get; set; }
        public DbSet<TablePohotovost> PohotovostiS { get; set; }
        public DbSet<TableTechnik> TechnikS { get; set; }
        public DbSet<DebugLogModel> LogS { get; set; }
    }
}
