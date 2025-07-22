namespace ModularDieselApplication.Domain.Entities
{
    public class Technik
    {
        public string ID { get;  set; } = null!;
        public bool Taken {get;  set;}
        public required User User {get;  set;}
        public required Firma Firma {get;  set;}
    }
}