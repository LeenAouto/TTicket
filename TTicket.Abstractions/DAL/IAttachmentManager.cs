using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.Abstractions.DAL
{
    public interface IAttachmentManager
    {
        Task<AttachmentModel> Get(Guid id);
        Task<PagedResponse<AttachmentModel>> GetList(AttachmentListRequestModel model);
        Task<AttachmentModel> Add(AttachmentModel attachment);
        Task<AttachmentModel> Update(AttachmentModel attachment);
        Task<AttachmentModel> Delete(AttachmentModel attachment);
    }
}
