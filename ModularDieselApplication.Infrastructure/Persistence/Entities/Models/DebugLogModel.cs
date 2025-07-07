using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ModularDieselApplication.Infrastructure.Persistence.Entities.Models
{
    public class DebugLogModel
    {
        [Key]
        public string IdLog { get; set; } = GetLogId();
        public DateTime TimeStamp { get; set; }
        [ForeignKey("Odstavka")]
        public string IdOdstavky { get; set; }
        public TableOdstavka odstavky { get; set; }
        [ForeignKey("Dieslovani")]
        public string IdDieslovani { get; set; }
        public TableDieslovani Dieslovani { get; set; }
        public required string LogMessage { get; set; }
        
        private static string GetLogId()
        {
            var random = new Random();
            var number = random.Next(0, 99999).ToString("D5");
            return $"LI-{number}";
        }
    }

}