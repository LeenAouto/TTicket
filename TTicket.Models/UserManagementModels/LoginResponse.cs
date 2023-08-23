namespace TTicket.Models.UserManagementModels
{
    public class LoginResponse : AuthBaseResponse
    {
        public bool IsAuthenticated { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresOn { get; set; }
    }
}
