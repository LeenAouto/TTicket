using TTicket.Models;

namespace TTicket.DAL.Interfaces
{
    public interface IAttachmentManager
    {
        Task<Attachment> Get(Guid id);
        Task<IEnumerable<Attachment>> GetAll();
        Task<IEnumerable<Attachment>> GetByAttachedToId(Guid id);
        Task<Attachment> Add(Attachment attachment);
        Attachment Update(Attachment attachment);
        Attachment Delete(Attachment attachment);
        Task<bool> IsValidAttachmentId(Guid id);
    }
}
