namespace TTicket.Models.DTOs
{
    public class AttachmentAddDto : AttachmentBaseDto
    {
        public Guid AttachedToId { get; set; } //Either a ticketId or a commentId
        public byte Attacher { get; set; }
    }
}
