namespace TTicket.Models
{
    public class Comment
    {
        public Guid Id { get; set; }
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }

        public Ticket Ticket { get; set; } = new Ticket();
        public User User { get; set; } = new User();
    }
}
