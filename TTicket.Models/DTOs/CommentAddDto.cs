namespace TTicket.Models.DTOs
{
    public class CommentAddDto : CommentBaseDto
    {
        public Guid TicketId { get; set; }
        //public Guid UserId { get; set; }
    }
}
