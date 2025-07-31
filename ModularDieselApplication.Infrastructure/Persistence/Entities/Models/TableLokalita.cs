using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableLokalita
    {
        [Key]
        public string ID { get; set; } = default!;
        public required string Nazev { get; set; }
        public required string Klasifikace { get; set; }
        public required string Adresa { get; set; }
        public int Baterie { get; set; }
        public bool DA { get; set; }
        public bool Zasuvka { get; set; }
        [ForeignKey("Region")]
        public required string RegionID { get; set; }
        public required TableRegion Region { get; set; }
        public virtual ICollection<TableOdstavka>? OdstavkyList { get; set; }
        [ForeignKey("Zdroj")]
        public string? ZdrojId { get; set; }
        public virtual TableZdroj? Zdroj { get; set; }
      
    }
}
