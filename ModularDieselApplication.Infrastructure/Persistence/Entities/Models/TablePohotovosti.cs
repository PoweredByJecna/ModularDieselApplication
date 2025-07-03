using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TablePohotovosti
    {
        [Key]
        public string ID { get; set; } = GenerateCustomId();
        public DateTime Zacatek { get; set; }
        public DateTime Konec { get; set; }

        [ForeignKey("User")]
        public required string IdUser { get; set; }
        public virtual required TableUser User { get; set; }

        [ForeignKey("Technik")]
        public required string IdTechnik { get; set; }
        public virtual required TableTechnici Technik { get; set; }       
        
        private static string GenerateCustomId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"PN-{number}";
        }   



    }
}