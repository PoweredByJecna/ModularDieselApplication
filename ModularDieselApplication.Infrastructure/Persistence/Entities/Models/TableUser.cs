using Microsoft.AspNetCore.Identity;
namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableUser : IdentityUser<string>
    {
        public required string Jmeno { get; set; }
        public required string Prijmeni { get; set; }
    }
}