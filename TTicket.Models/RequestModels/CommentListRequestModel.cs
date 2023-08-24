using System.ComponentModel.DataAnnotations;

namespace TTicket.Models.RequestModels
{
    public class CommentListRequestModel : BaseListRequestModel
    {
        [Required]
        public Guid TicketId { get; set; } = default(Guid);
    }
}
