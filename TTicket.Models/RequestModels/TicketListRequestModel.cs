namespace TTicket.Models.RequestModels
{
    public class TicketListRequestModel : BaseListRequestModel
    {
        public Guid? UserId { get; set; }
        public Guid? SupportId { get; set; }
        public Guid? ProductId { get; set; }
        public string? Name { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public TicketStatus? Status { get; set; }
    }
}
