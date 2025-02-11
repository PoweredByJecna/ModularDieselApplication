using Microsoft.AspNetCore.Identity;
namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableUser : IdentityUser
    {
        public string Jmeno { get; set; }
        public string Prijmeni { get; set; }
    }
}