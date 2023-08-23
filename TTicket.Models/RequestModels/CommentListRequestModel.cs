namespace TTicket.Models.RequestModels
{
    public class CommentListRequestModel : BaseListRequestModel
    {
        public Guid TicketId { get; set; }
        //public Guid? UserId { get; set; }
        //public DateTime? CreatedDate { get; set; }
    }
}
