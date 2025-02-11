public class Lokalita
{
    public int ID {get; private set;}
    public string Nazev {get; private set;}
    public string Klasifikace {get; private set;}
    public string Adresa {get; private set;}
    public int Baterie{get; private set;}
    public bool DA{get; private set;}
    public bool Zasuvka{get; private set;}
    public Region Region {get; private set;}
    public Zdroj? Zdroj {get; private set;}
}