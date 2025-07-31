using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{

    public class TableOdstavka
    {
        [Key]
        public string ID { get; set; } = default!;
        public required string Distributor { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public required string Popis { get; set; }

        [Column("LokalitaID")]
        public string LokalitaID { get; set; } = null!;

        [ForeignKey(nameof(LokalitaID))]
        public virtual required TableLokalita Lokality { get; set; }
    }
}