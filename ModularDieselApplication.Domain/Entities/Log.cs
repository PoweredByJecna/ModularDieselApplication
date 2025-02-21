
namespace ModularDieselApplication.Domain.Entities
{
    public class Log
    {
        public int IdLog { get; set; }
        public DateTime TimeStamp { get; set; }
        public required string EntityName { get; set; }
        public int EntityId { get; set; }
        public string LogMessage { get; set; } = string.Empty;
    }
}