namespace TTicket.Models
{
    public class Ticket
    {
        public Guid Id { get; set; }
        public Guid ClientId { get; set; }
        public Guid? SupportId { get; set; }
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TicketStatus Status { get; set; }

        public User Client { get; set; } = new User();
        public User Support { get; set; } = new User();
        public Product Product { get; set; } = new Product();
    }

    public enum TicketStatus : byte
    {
        New = 1,
        Assigned = 2,
        Closed = 3
    }
}
