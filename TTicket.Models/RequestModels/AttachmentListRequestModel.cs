namespace TTicket.Models.RequestModels
{
    public class AttachmentListRequestModel : BaseListRequestModel
    {
        public Guid AttachedToId { get; set; } //Either a ticketId or a commentId
    }
}
