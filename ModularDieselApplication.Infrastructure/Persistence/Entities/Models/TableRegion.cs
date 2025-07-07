using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableRegion
    {   
        [Key]
        public string ID { get; set; } = Guid.NewGuid().ToString();
        public required string Nazev{get;set;}
        
        [ForeignKey("Firma")]
        public required string FirmaID {get;set;}
        public virtual required TableFirma Firma {get;set;}
    }
}