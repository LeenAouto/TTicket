using Microsoft.AspNetCore.Http;

namespace TTicket.Models.DTOs
{
    public class AttachmentAddDto 
    {
        public Guid AttachedToId { get; set; } //Either a ticketId or a commentId
        public IFormFile AttachmentFile { get; set; }
    }
}
