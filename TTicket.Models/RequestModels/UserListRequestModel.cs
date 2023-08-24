namespace TTicket.Models.RequestModels
{
    public class UserListRequestModel : BaseListRequestModel
    {
        public string? Username { get; set; } 
        public string? Email { get; set; } 
        public string? MobilePhone { get; set; } 
        public string? FirstName { get; set; } 
        public string? LastName { get; set; } 
        public UserType? TypeUser { get; set; }
        public UserStatus? StatusUser { get; set; }
    }
}
