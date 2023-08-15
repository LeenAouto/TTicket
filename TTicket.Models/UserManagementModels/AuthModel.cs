namespace TTicket.Models.UserManagementModels
{
    public class AuthModel
    {
        //User info
        public Guid Id { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public byte TypeUser { get; set; }
        public byte StatusUser { get; set; }

        //Auth info
        public string Message { get; set; } = string.Empty;
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}
