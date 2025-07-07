using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TablePohotovost
    {
        [Key]
        public string ID { get; set; } = GenerateCustomId();
        public DateTime Zacatek { get; set; }
        public DateTime Konec { get; set; }
        
        [ForeignKey("Technik")]
        public required string IdTechnik { get; set; }
        public virtual required TableTechnik Technik { get; set; }       
        
        private static string GenerateCustomId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"PN-{number}";
        }   



    }
}