namespace ModularDieselApplication.Domain.Entities
{
    public class User
    {
        public string Id { get; set; } = null!;
        public string UserName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string Jmeno { get; set; } = null!;
        public string Prijmeni { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}