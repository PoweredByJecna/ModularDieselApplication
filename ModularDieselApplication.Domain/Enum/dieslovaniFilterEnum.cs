
namespace ModularDieselApplication.Domain.Enum
{
    public enum DieslovaniFilterEnum
    {
        AllTable,
        RunningTable,
        UpcomingTable,
        EndTable,
        TrashTable
    }

    public enum RegionyFilterEnum
    {
        Praha,
        SeverniMorava,
        JizniMorava,
        ZapadniCechy,
        JizniCechy,
        SeverniCechy
    }

    public enum IsDieselRequiredEnum
    {
        Yes,
        Baterie,
        Zasuvka,
        Agregat,
        Priorita
    }

    public enum DieslovaniFieldEnum
    {
        ID,
        Odstavka,
        Technik,
        Vstup,
        Odchod
    }

    public enum GetTechnikEnum
    {
        ById,
        ByFirmaId,
        ByUserId,
        All,
        fitkivniTechnik
    }

   
}