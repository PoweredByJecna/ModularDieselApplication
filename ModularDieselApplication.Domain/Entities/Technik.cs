namespace ModularDieselApplication.Domain.Entities
{
    public class Technik
    {
        public string? ID {get;  set;}
        public bool Taken {get;  set;}
        public User? User {get; private set;}
        public Firma? Firma {get; private set;}
    }
}