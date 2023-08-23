namespace TTicket.Models.UserManagementModels
{
    public class AuthBaseResponse
    {
        //user properties
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public UserType TypeUser { get; set; }
        public UserStatus StatusUser { get; set; }

        //auth properties
        public string Message { get; set; } = string.Empty;
    }
}
