using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableRegiony
    {   
        [Key]
        public int ID{get;set;}
        public string Nazev{get;set;}
        
        [ForeignKey("Firma")]
        public int  FirmaID {get;set;}
        public virtual TableFirma Firma {get;set;}
    }
}