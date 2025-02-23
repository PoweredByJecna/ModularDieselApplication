

using System.Security.Principal;

namespace ModularDieselApplication.Domain.Entities
{
    public class Pohotovosti
    {
        public int ID { get; set; }
        public DateTime Zacatek { get; set; }
        public DateTime Konec { get; set; }
        public required string IdUser { get; set; }
        public User User { get; set; }
        public string IdTechnik { get; set; }
        public Technik Technik { get; set; }
    }
}
