using TTicket.Models;

namespace TTicket.DAL.Interfaces
{
    public interface ICommentManager
    {
        Task<Comment> Get(Guid id);
        Task<IEnumerable<Comment>> GetAll();
        Task<IEnumerable<Comment>> GetByUserId(Guid id);
        Task<IEnumerable<Comment>> GetByTicketId(Guid id);
        Task<Comment> Add(Comment comment);
        Comment Update(Comment comment);
        Comment Delete(Comment comment);
        Task<bool> IsValidCommentId(Guid id);
    }
}
