

using System.Security.Principal;

namespace ModularDieselApplication.Domain.Entities
{
    public class Pohotovosti
    {
        public string ID { get; set; } = null!;
        public DateTime Zacatek { get; set; }
        public DateTime Konec { get; set; }
        public required string IdUser { get; set; }
        public required User User { get; set; }
        public required string IdTechnik { get; set; }
        public required Technik Technik { get; set; }
    }
}
