using System;
using System.ComponentModel.DataAnnotations;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models {

public class TableZdroj
{
    [Key]
    public string ID { get; set; } = default!;
    public required string Nazev { get; set; } 
    public double Odber { get; set; }   
}
}
