using System;
using System.ComponentModel.DataAnnotations;

namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models {

public class TableZdroj
{
    [Key]
    public int Id { get; set; }
    public string? Nazev { get; set; } 
    public double? Odber { get; set; }   
}
}
