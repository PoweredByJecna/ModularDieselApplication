namespace ModularDieselApplication.Domain.Entities 
{
    public class Region
    {
        public string ID {get;  set;} = null!;
        public string Nazev {get;  set;} = null!;
        public required Firma Firma {get;  set;}
    }
}