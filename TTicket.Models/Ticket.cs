namespace TTicket.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; } //client
        public Guid? SupportId { get; set; } = null;
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TicketStatus Status { get; set; }

        public User User { get; set; } //client
        public User? Support { get; set; }
        public Product Product { get; set; }
    }

    public enum TicketStatus : byte
    {
        New = 1,
        Assigned = 2,
        Closed = 3
    }
}
