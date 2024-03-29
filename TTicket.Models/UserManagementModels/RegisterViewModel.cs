﻿using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.UserManagementModels
{
    public class RegisterViewModel
    {
        [Required, StringLength(256)]
        public string Username { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string Password { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string Email { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string MobilePhone { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string FirstName { get; set; } = string.Empty;

        [Required, StringLength(256)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public string? Address { get; set; }


        public IFormFile? Image { get; set; }
    }
}
