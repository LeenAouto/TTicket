using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.DTOs
{
    public class UserUpdateDto
    {
        [MaxLength(256)]
        public string Username { get; set; } = string.Empty;
        [MaxLength(256)]
        public string Password { get; set; } = string.Empty;
        [MaxLength(256)]
        public string Email { get; set; } = string.Empty;
        [MaxLength(256)]
        public string MobilePhone { get; set; } = string.Empty;
        [MaxLength(256)]
        public string FirstName { get; set; } = string.Empty;
        [MaxLength(256)]
        public string LastName { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; } = default;
        [MaxLength(256)]
        public string? Address { get; set; }
        public UserType TypeUser { get; set; } = default;
        public UserStatus StatusUser { get; set; } = default;
    }
}
