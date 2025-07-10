

namespace ModularDieselApplication.Domain.Entities
{
    public class Odstavka
    {
        public string ID { get; set; } = null!;
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public string? Popis { get; set; }
        public string Distributor { get; set; } = null!;
        public required Lokalita Lokality { get; set; }
    }
}
