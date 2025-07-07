
namespace ModularDieselApplication.Domain.Entities
{
    public class Log
    {
        public required string IdLog;
        public DateTime TimeStamp { get; set; }
        public string LogMessage { get; set; } = string.Empty;
    }
}