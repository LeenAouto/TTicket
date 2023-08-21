namespace TTicket.Models.PresentationModels
{
    public class UserModel
    {
        public UserModel() { }
        public UserModel(User user, int? ticketCount = null)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            MobilePhone = user.MobilePhone;
            FirstName = user.FirstName;
            LastName = user.LastName;
            DateOfBirth = user.DateOfBirth;
            Address = user.Address;
            TypeUser = user.TypeUser;
            StatusUser = user.StatusUser;

            TicketCount = ticketCount;
        }

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

        public int? TicketCount { get; set; } = null;
    }
}
