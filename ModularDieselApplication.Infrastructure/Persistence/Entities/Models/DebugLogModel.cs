using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;


namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class DebugLogModel
    {
        [Key]
        public string IdLog { get; set; } = GetLogId();
        public DateTime TimeStamp { get; set; }
        public required string EntityName { get; set; }
        public required string EntityId { get; set; }
        public required string LogMessage { get; set; }
        
        private static string GetLogId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"LI-{number}";
        }
    }

}