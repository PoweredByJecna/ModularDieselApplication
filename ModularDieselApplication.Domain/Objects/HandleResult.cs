using ModularDieselApplication.Domain.Entities;

namespace ModularDieselApplication.Domain.Objects
{
    public class HandleResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public Dieslovani? Dieslovani { get; set; }
        public Odstavka? Odstavka { get; set; }
        public string EmailResult { get; set; } = "";
        public string Duvod { get; set; } = "";
        public string Color { get; set; } = "";
    }
}