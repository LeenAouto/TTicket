using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

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
                var comment = await _context.Comment.
                    Where(c => c.Id == id).
                    Select(c => new CommentModel(c)).
                    SingleOrDefaultAsync();

                if (comment != null)
                {
                    var attachments = await _context.Attachment.
                        Where(a => a.AttachedToId == comment.Id).
                        Select(a => new AttachmentModel(a)).
                    ToListAsync();

                    comment.AttachmentsList = attachments;
                }

                return comment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<PagedResponse<CommentModel>> GetList(CommentListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;

                var query = _context.Comment.
                    Where(c => c.TicketId == model.TicketId);

                var totalCount = await query.CountAsync();

                var comments = await query.
                    OrderByDescending(c => c.CreatedDate).
                    Skip(skip).
                    Take(model.PageSize).
                    Select(c => new CommentModel(c)).
                    ToListAsync();

                return new PagedResponse<CommentModel>(comments, totalCount);
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
