namespace TTicket.Models.RequestModels
{
    public class UserRequestModel
    {
        public Guid? Id { get; set; }
        public string? Identity { get; set; }
        public UserType? TypeUser { get; set; }
    }
}
