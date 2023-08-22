using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.DTOs
{
    public class UserUpdateDto
    {
        [MaxLength(256)]
        public string? Username { get; set; }
        [MaxLength(256)]
        public string? Password { get; set; }
        [MaxLength(256)]
        public string? Email { get; set; }
        [MaxLength(256)]
        public string? MobilePhone { get; set; }
        [MaxLength(256)]
        public string? FirstName { get; set; }
        [MaxLength(256)]
        public string? LastName { get; set; }
        public DateTime? DateOfBirth { get; set; }
        [MaxLength(256)]
        public string? Address { get; set; }

        public IFormFile? Image { get; set; }
    }
}
