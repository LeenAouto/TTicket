using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;

namespace TTicket.DAL.Managers
{
    public class CommentManager : ICommentManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CommentManager> _logger;

        public CommentManager(ApplicationDbContext context, ILogger<CommentManager> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<CommentModel> Get(Guid id)
        {
            try
            {
                var comment = await _context.Comment.SingleOrDefaultAsync(c => c.Id == id);

                return comment != null ? new CommentModel(comment) : null;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<CommentModel>> GetList(CommentListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;
                var comments = await _context.Comment.
                    Where(c => (c.TicketId == model.TicketId || model.TicketId == null)
                            &&(c.UserId == model.UserId || model.UserId == null)
                            &&(c.CreatedDate == model.CreatedDate || model.CreatedDate == null)
                            ).
                    OrderByDescending(c => c.CreatedDate).
                    Skip(skip).
                    Take(model.PageSize).
                    ToListAsync();

                var commentsList = new List<CommentModel>();
                if(comments.Any())
                    foreach(var comment in comments)
                        commentsList.Add(new CommentModel(comment));

                return commentsList;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<CommentModel> Add(CommentModel comment)
        {
            try
            {
                var c = new Comment
                {
                    TicketId = comment.TicketId,
                    UserId = comment.UserId,
                    Content = comment.Content,
                    CreatedDate = comment.CreatedDate
                };


                await _context.Comment.AddAsync(c);
                _context.SaveChanges();

                return new CommentModel(c);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<CommentModel> Update(CommentModel comment)
        {
            try
            {
                var c = await _context.Comment.SingleAsync(c => c.Id == comment.Id);

                c.TicketId = comment.TicketId;
                c.UserId = comment.UserId;
                c.Content = comment.Content;
                c.CreatedDate = comment.CreatedDate;

                _context.Comment.Update(c);
                _context.SaveChanges();

                return new CommentModel(c);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<CommentModel> Delete(CommentModel comment)
        {
            try
            {
                var c = await _context.Comment.SingleAsync(c => c.Id == comment.Id);

                _context.Comment.Remove(c);
                _context.SaveChanges();

                return new CommentModel(c);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
