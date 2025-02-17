using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TablePohotovosti
    {
        [Key]
        public int ID{get;set;}
        public DateTime Zacatek {get;set;}
        public DateTime Konec {get;set;}

        [ForeignKey("User")]
        public string IdUser{get;set;}
        public virtual TableUser User {get;set;}

        [ForeignKey("Technik")]
        public string? IdTechnik {get;set;}
        public virtual TableTechnici? Technik {get;set;}       



    }
}