
namespace ModularDieselApplication.Domain.Enum
{
    public enum DieslovaniOdstavkaFilterEnum
    {
        AllTable,
        RunningTable,
        UpcomingTable,
        EndTable,
        TrashTable,
        OD
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

    public enum ActionFilter
    {
        Vstup,
        Odchod,
        Delete,
        CallDA,
        ChangeTimeOdchod,
        ChangeTimeVstup,
        ChangeTimeZactek,
        ChangeTimeKonec,
        take,
        zacatek,
        konec,
        Create
    }

    public enum TechnikFilterEnum
    {
        taken,
        ID
    }
    public enum ServiceFilterEnum
    {
        Dieslovani,
        Odstavka
    }
    public enum OdstavkaOption
    {
        now,
        time,
        classic,
    }
    public enum GetDA
    {
        ById,
        BYRegion,
        ByLokalita,
        ByOdstavkaId
    }
    public enum GetOdstavka
    {
        ById,
        ByDaId,
    }
    public enum GetLokalita
    {
        ById,
        ByNazev,
        ByRegion
    }




}

