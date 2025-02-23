

namespace ModularDieselApplication.Domain.Entities
{
    public class Odstavka
    {
        public int ID { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public required string Popis { get; set; }
        public required string Distributor { get; set; }
        public required Lokalita Lokality { get; set; }
    }
}
