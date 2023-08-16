using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TTicket.Abstractions.DAL;
using TTicket.Models;

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

        public async Task<Comment> Get(Guid id)
        {
            try
            {
                return await _context.Comment.
                    Include(c => c.TicketId).
                    Include(c => c.UserId).
                    SingleOrDefaultAsync(c => c.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Comment>> GetAll()
        {
            try
            {

                return await _context.Comment.
                    OrderBy(c => c.CreatedDate).
                    Include(c => c.TicketId).
                    Include(c => c.UserId).
                    ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Comment>> GetByTicketId(Guid id)
        {
            try
            {
                return await _context.Comment.
                Where(c => c.TicketId == id).
                OrderBy(c => c.CreatedDate).
                Include(c => c.TicketId).
                Include(c => c.UserId).
                ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Comment>> GetByUserId(Guid id)
        {
            try
            {
                return await _context.Comment.
                Where(c => c.UserId == id).
                OrderBy(c => c.CreatedDate).
                Include(c => c.TicketId).
                Include(c => c.UserId).
                ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<Comment> Add(Comment comment)
        {
            try
            {
                await _context.Comment.AddAsync(comment);
                _context.SaveChanges();
                return comment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Comment Update(Comment comment)
        {
            try
            {
                _context.Comment.Update(comment);
                _context.SaveChanges();
                return comment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Comment Delete(Comment comment)
        {
            try
            {
                _context.Comment.Remove(comment);
                _context.SaveChanges();
                return comment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<bool> IsValidCommentId(Guid id)
        {
            try
            {
                return await _context.Comment.AnyAsync(c => c.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
