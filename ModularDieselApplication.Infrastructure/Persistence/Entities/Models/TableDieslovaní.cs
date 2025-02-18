using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableDieslovani
    {
        [Key]
        public int ID{get;set;}
        public DateTime? Vstup {get;set;}
        public DateTime? Odchod {get;set;}
        
        [ForeignKey("Odstavka")]
        public int IDodstavky {get;set;}
        public virtual required TableOdstavky Odstavka {get;set;}
        
        [ForeignKey("Technik")]
        public required string IdTechnik {get;set;}
        public virtual required TableTechnici Technik {get;set;}
     
    }
}