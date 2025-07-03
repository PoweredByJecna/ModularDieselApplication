using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableFirma
    {
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public required string Nazev { get; set; }       
    }
}