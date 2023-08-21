using Microsoft.AspNetCore.Http;

namespace TTicket.Models.DTOs
{
    public class AttachmentUpdateDto 
    {
        public IFormFile AttachmentFile { get; set; }
    }
}
