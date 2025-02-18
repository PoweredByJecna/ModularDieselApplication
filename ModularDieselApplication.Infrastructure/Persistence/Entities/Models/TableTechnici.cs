using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
   
    public class TableTechnici
    {
        [Key]
        public required string ID{get;set;}
        public bool Taken{get;set;}=false;

        [ForeignKey("Firma")]
        public int FirmaId{get;set;}
        public virtual required TableFirma Firma {get;set;}

        [ForeignKey("User")]
        public required string IdUser{get;set;}
        public virtual required TableUser User {get;set;}
        public virtual ICollection<TableDieslovani>? DieslovaniList {get;set;}
    }

   }