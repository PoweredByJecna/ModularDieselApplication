using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableLokality
    {
        

        [Key]
        public int ID { get; set; }
        public required string Nazev { get; set; }
        public required string Klasifikace { get; set; }
        public required string Adresa { get; set; }
        public int Baterie { get; set; }
        public bool DA { get; set; }
        public bool Zasuvka { get; set; }



        [ForeignKey("Region")]
        public int RegionID {get;set;}
        public TableRegiony Region {get;set;}
        public virtual ICollection<TableOdstavky>? OdstavkyList { get; set; }
        
        [ForeignKey("Zdroj")]
        public int? ZdrojId { get; set; }
        public virtual TableZdroj? Zdroj { get; set; }
      
    }
}
