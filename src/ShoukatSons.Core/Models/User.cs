namespace ShoukatSons.Core.Models
{
    public enum UserRole
    {
        Admin = 1,
        Cashier = 2,
        Manager = 3
    }

    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public UserRole Role { get; set; } = UserRole.Cashier;
        public bool IsActive { get; set; } = true;
    }
}