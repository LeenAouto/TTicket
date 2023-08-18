namespace TTicket.Models.RequestModels
{
    public class AttachmentRequestModel
    {
        public Guid? Id { get; set; }
        public string? FileName { get; set; } = string.Empty;
    }
}
