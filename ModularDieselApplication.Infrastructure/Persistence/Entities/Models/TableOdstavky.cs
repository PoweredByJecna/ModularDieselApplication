using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableOdstavky
    {
        [Key]
        public int ID { get; set; }
        public string Distributor { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public string Popis { get; set; }

        [Column("LokalitaID")]
        public int LokalitaID { get; set; }

        [ForeignKey(nameof(LokalitaID))]
        public virtual TableLokality Lokality { get; set; }
        public virtual ICollection<TableDieslovani>? DieslovaniList {get;set;}
  
    }
}
