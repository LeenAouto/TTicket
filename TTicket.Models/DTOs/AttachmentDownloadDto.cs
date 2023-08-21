namespace TTicket.Models.DTOs
{
    public class AttachmentDownloadDto
    {
        public Guid AttachedToId { get; set; } //Either a ticketId or a commentId
        public string FileName { get; set; } = string.Empty;
    }
}
