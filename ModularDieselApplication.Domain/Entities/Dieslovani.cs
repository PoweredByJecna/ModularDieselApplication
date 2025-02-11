

namespace ModularDieselApplication.Domain.Entities
{
    public class Dieslovani
    {
        public int ID { get; set; }
        public Odstavka Odstavka { get; set; }
        public Technik Technik { get; set; }

        public DateTime? Vstup { get; set; }
        public DateTime? Odchod { get; set; }

        public bool ZadanVstup => Vstup.HasValue;
        public bool ZadanOdchod => Odchod.HasValue;

        // Other properties and methods...
    }
}