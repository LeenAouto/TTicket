using TTicket.Models;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface ICommentManager
    {
        Task<Comment> Get(Guid id);
        Task<IEnumerable<Comment>> GetList(CommentListRequestModel model);
        Task<Comment> Add(Comment comment);
        Comment Update(Comment comment);
        Comment Delete(Comment comment);



        //Task<IEnumerable<Comment>> GetByUserId(Guid id);
        //Task<IEnumerable<Comment>> GetByTicketId(Guid id);
        //Task<bool> IsValidCommentId(Guid id);
    }
}
