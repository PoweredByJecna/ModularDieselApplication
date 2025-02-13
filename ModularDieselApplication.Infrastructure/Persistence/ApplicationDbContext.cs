using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ModularDieselApplication.Infrastructure.Persistence.Entities.Models;

namespace ModularDieselApplication.Infrastructure.Persistence
{
    public class ApplicationDbContext : IdentityDbContext<TableUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.HasDefaultSchema("Identity");
            base.OnModelCreating(builder);
            builder.Entity<TableUser>().ToTable("User");
            builder.Entity<IdentityRole>().ToTable("Role");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            // MAPPING TABULEK
            builder.Entity<TableLokality>().ToTable("LokalityTable", schema: "Data");
            builder.Entity<TableOdstavky>().ToTable("OdstavkyTable", schema: "Data");
            builder.Entity<TableDieslovani>().ToTable("TableDieslovani", schema: "Data");
            builder.Entity<TableFirma>().ToTable("TableFirma", schema: "Data");
            builder.Entity<TableRegiony>().ToTable("TableRegiony", schema: "Data");
            builder.Entity<TablePohotovosti>().ToTable("TablePohotovosti", schema: "Data");
            builder.Entity<TableTechnici>().ToTable("TableTechnici", schema: "Data");
            builder.Entity<DebugLogModel>().ToTable("DebugModel", schema: "Data");
            builder.Entity<TableZdroj>().ToTable("TableZdroj", schema: "Data");


            builder.Entity<TableOdstavky>()
                .HasOne(o => o.Lokality) // Navigační vlastnost v Odstavky
                .WithMany(l => l.OdstavkyList) // Kolekce odstávek v Lokality
                .HasForeignKey(o => o.LokalitaID) // Cizí klíč v Odstavky
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TableDieslovani>()
                .HasOne(o=>o.Odstavka)
                .WithMany(I=>I.DieslovaniList)
                .HasForeignKey(o=>o.IDodstavky)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<TableDieslovani>()
                .HasOne(o=>o.Technik)
                .WithMany(i=>i.DieslovaniList)
                .HasForeignKey(o=>o.IdTechnik);
                


            
        }

   
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
