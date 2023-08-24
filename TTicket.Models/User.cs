namespace TTicket.Models
{
    public class User
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty; 
        public string LastName { get; set; } = string.Empty; 
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public UserType TypeUser { get; set; }
        public UserStatus StatusUser { get; set; }
    }

    public enum UserType : byte
    {
        Manager = 1,
        Support = 2,
        Client = 3
    }

    public enum UserStatus : byte
    {
        Active = 1, 
        Inactive = 2
    }
}