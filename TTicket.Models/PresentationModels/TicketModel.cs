namespace TTicket.Models.PresentationModels
{
    public class TicketModel
    {
        public TicketModel() { }
        public TicketModel(Ticket ticket)
        {
            Id = ticket.Id;
            UserId = ticket.UserId;
            SupportId = ticket.SupportId;
            ProductId = ticket.ProductId;
            Name = ticket.Name;
            Content = ticket.Content;
            CreatedDate = ticket.CreatedDate;
            UpdatedDate = ticket.UpdatedDate;
            Status = ticket.Status;
        }
        public Guid Id { get; set; }
        public Guid UserId { get; set; } //client
        public Guid? SupportId { get; set; } = null;
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TicketStatus Status { get; set; }
    }
}
