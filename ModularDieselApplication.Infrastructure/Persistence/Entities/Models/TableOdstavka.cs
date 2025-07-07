using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{

    public class TableOdstavka
    {
        [Key]
        public string ID { get; set; } = GenerateCustomId();
        public required string Distributor { get; set; }
        public DateTime Od { get; set; }
        public DateTime Do { get; set; }
        public required string Popis { get; set; }

        [Column("LokalitaID")]
        public string LokalitaID { get; set; }

        [ForeignKey(nameof(LokalitaID))]
        public virtual required TableLokalita Lokality { get; set; }
        public virtual ICollection<TableDieslovani>? DieslovaniList { get; set; }
        
        private static string GenerateCustomId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"OT-{number}";
        }
  
    }
}
