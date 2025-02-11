using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class TableFirma
    {
        [Key]
        public int ID{get;set;}
        public string Nazev{get;set;}       
    }
}