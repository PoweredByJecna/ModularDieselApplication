namespace ModularDieselApplication.Domain.Entities 
{
    public class Lokalita
    {
        public int ID {get; set;}
        public string? Nazev {get; set;}
        public string? Klasifikace {get; set;}
        public string? Adresa {get; set;}
        public int Baterie{get; set;}
        public bool DA{get; set;}
        public bool Zasuvka{get; set;}
        public Region Region {get; set;}
        public Zdroj? Zdroj {get; set;}
    }
}