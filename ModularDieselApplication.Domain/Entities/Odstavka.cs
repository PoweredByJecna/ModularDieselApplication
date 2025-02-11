

namespace ModularDieselApplication.Domain.Entities
{
    public class Odstavka
    {
        public int ID { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public string Popis { get; set; }
        public string Distributor { get; set; }
        public Lokalita Lokality { get; set; }
    }
}
