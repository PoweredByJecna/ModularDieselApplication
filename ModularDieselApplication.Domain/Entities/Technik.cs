using ModularDieselApplication.Domain.Enum;

namespace ModularDieselApplication.Domain.Entities
{
    public class Technik
    {
        public string ID { get; private set; }
        public bool Taken { get; private set; }
        public User? User { get; }
        public Firma? Firma { get; }

        public Technik(string id, User? user, Firma? firma)
        {
            ID = id;
            User = user;
            Firma = firma;
            Taken = false;
        }
        public void Nastav(TechnikFilterEnum filter, object value)
        {
            switch (filter)
            {
                case TechnikFilterEnum.taken:
                    Taken = (bool)value;
                    break;
                default:
                    throw new ArgumentException("Invalid filter for Technik.");
            }
        }

    }





}