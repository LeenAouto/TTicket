namespace TTicket.Models.DTOs
{
    public class TicketAddDto //: TicketBaseDto
    {
        public Guid ProductId { get; set; }
        public string Content { get; set; } = string.Empty;
        public Guid UserId { get; set; }
    }
}
