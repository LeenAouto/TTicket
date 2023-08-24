namespace TTicket.Models.RequestModels
{
    public class CommentListRequestModel : BaseListRequestModel
    {
        public Guid TicketId { get; set; }
    }
}
