

namespace ModularDieselApplication.Domain.Entities
{
    public class Dieslovani
    {
        public int ID { get; set; }
        public required Odstavka Odstavka { get; set; }
        public required Technik Technik { get; set; }
        public DateTime? Vstup { get; set; }
        public DateTime? Odchod { get; set; }
    }
}