using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface ICommentManager
    {
        Task<CommentModel> Get(Guid id);
        Task<IEnumerable<CommentModel>> GetList(CommentListRequestModel model);
        Task<CommentModel> Add(CommentModel comment);
        Task<CommentModel> Update(CommentModel comment);
        Task<CommentModel> Delete(CommentModel comment);
    }
}
