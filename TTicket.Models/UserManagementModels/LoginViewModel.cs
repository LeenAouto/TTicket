using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.UserManagementModels
{
    public class LoginViewModel
    {
        [Required]
        public string Identity { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
