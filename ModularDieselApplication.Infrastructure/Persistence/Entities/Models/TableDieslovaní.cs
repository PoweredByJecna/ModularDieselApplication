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
        public virtual TableOdstavky Odstavka {get;set;}
        
        [ForeignKey("Technik")]
        public string IdTechnik {get;set;}
        public virtual TableTechnici Technik {get;set;}
     
    }
}