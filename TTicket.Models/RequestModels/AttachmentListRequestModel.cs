namespace TTicket.Models.RequestModels
{
    public class AttachmentListRequestModel : BaseListRequestModel
    {
        public Guid? AttachedToId { get; set; } //Either a ticketId or a commentId
        public string? FileName { get; set; } 
        public AttacherType? Attacher { get; set; }
    }
}
