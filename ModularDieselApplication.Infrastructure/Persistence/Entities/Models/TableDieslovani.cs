using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableDieslovani
    {
        [Key]
        public string ID { get; set; } = GenerateCustomId();
        public DateTime Vstup { get; set; }
        public DateTime Odchod { get; set; }
        [ForeignKey("Odstavka")]
        public required string IdOdstavky { get; set; }
        public virtual required TableOdstavka Odstavka { get; set; }

        [ForeignKey("Technik")]
        public required string IdTechnik { get; set; }
        public virtual required TableTechnik Technik { get; set; }

        private static string GenerateCustomId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"DT-{number}";
        }
    }
    
    
}