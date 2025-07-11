namespace ModularDieselApplication.Domain.Entities
{
    public class User
    {
        public string Id { get; private set; } = null!;
        public string UserName { get; private set; } = null!;
        public string Email { get; private set; } = null!;
        public string Jmeno { get; private set; } = null!;
        public string Prijmeni { get; private set; } = null!;
        public string PhoneNumber { get; private set; } = null!;
    }
}