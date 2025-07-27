
using ModularDieselApplication.Domain.Enum;
using ModularDieselApplication.Domain.Objects;



namespace ModularDieselApplication.Domain.Entities
{
    public class Dieslovani<T>
    {
        public string ID { get; private set; }
        public required Odstavka Odstavka { get; set; }
        public required Technik Technik { get; set; }
        public DateTime Vstup { get; private set; }
        public DateTime Odchod { get; private set; }
        public void Nastav(DieslovaniFieldEnum field, Dieslovani<T> value)
        {
            switch (field)
            {
                case DieslovaniFieldEnum.ID:
                    ID = value.ID;
                    break;
                case DieslovaniFieldEnum.Vstup:
                    Vstup = value.Vstup;
                    break;
                case DieslovaniFieldEnum.Odchod:
                    if (value.Odchod < value.Vstup)
                    {
                        throw new ArgumentException("Odchod nemůže být dřív než Vstup.");
                    }
                    Odchod = value.Odchod;
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(field), field, null);
            }
        }
        public Dieslovani(string id, Odstavka odstavka = default!, Technik technik =default!, DateTime vstup = default, DateTime odchod = default)
        {
            ID = string.IsNullOrEmpty(id) ? GenerateCustomId() : id;
            Odstavka = odstavka;
            Technik = technik;
            Vstup = vstup;
            Odchod = odchod;
        }
        private static string GenerateCustomId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"DT-{number}";
        }


    }
    public class Dieslovani : Dieslovani<Object>
    {
        public Dieslovani(string id, Odstavka odstavka = default!, Technik technik =default! , DateTime vstup = default, DateTime odchod = default)
            : base(id, odstavka, technik, vstup, odchod)
        {
        }
    }

   
    
}