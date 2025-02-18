namespace ModularDieselApplication.Application.Interfaces
{
    public interface IDatabaseCleaner
    {
        Task CleanOutdatedRecords();
    }
}
