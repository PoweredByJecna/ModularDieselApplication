
using System.Reflection.Metadata;
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
        public void Nastav(DieslovaniFieldEnum field, object value)
        {
            switch (field)
            {
                case DieslovaniFieldEnum.ID:
                    ID = value as string ?? throw new ArgumentException("ID musí být řetězec.");
                    break;
                case DieslovaniFieldEnum.Vstup:
                    Vstup = value as DateTime? ?? throw new ArgumentException("Vstup musí být platný datum a čas.");
                    break;
                case DieslovaniFieldEnum.Odchod:
                    var odchod = value as DateTime?;
                    if (odchod < Vstup)
                    {
                        throw new ArgumentException("Odchod nemůže být dřív než Vstup.");
                    }
                    Odchod = value as DateTime? ?? throw new ArgumentException("Odchod musí být platný datum a čas.");
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(field), field, null);
            }
        }
        public Dieslovani(string? id = null, Odstavka odstavka = default!, Technik technik =default!, DateTime vstup = default, DateTime odchod = default)
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
    public class Dieslovani : Dieslovani<object>
    {
        public Dieslovani(string? id = null, Odstavka odstavka = default!, Technik technik =default! , DateTime vstup = default, DateTime odchod = default)
            : base(id, odstavka, technik, vstup, odchod)
        {
        }
    }

   
    
}