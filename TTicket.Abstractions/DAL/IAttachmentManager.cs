using TTicket.Models;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface IAttachmentManager
    {
        Task<Attachment> Get(AttachmentRequestModel model);
        Task<IEnumerable<Attachment>> GetList(AttachmentListRequestModel model);
        Task<Attachment> Add(Attachment attachment);
        Attachment Update(Attachment attachment);
        Attachment Delete(Attachment attachment);



        //Task<IEnumerable<Attachment>> GetByAttachedToId(Guid id);
        //Task<bool> IsValidAttachmentId(Guid id);
    }
}
