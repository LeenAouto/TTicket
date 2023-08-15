using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Net.Sockets;
using TTicket.DAL.Interfaces;
using TTicket.Models;

namespace TTicket.DAL.Managers
{
    public class AttachmentManager : IAttachmentManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AttachmentManager> _logger;

        public AttachmentManager(ApplicationDbContext context, ILogger<AttachmentManager> logger)
        {
            _context = context;
            _logger = logger;
        }


        public async Task<Attachment> Get(Guid id)
        {
            try
            {
                return await _context.Attachment.SingleOrDefaultAsync(a => a.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Attachment>> GetAll()
        {
            try
            {
                return await _context.Attachment.
                    OrderBy(a => a.Id).
                    ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Attachment>> GetByAttachedToId(Guid id)
        {
            try
            {
                return await _context.Attachment.
                    Where(a => a.AttachedToId == id).
                    OrderBy(a => a.Id).
                    ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<Attachment> Add(Attachment attachment)
        {
            try
            {
                await _context.Attachment.AddAsync(attachment);
                _context.SaveChanges();
                return attachment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Attachment Update(Attachment attachment)
        {
            try
            {
                _context.Attachment.Update(attachment);
                _context.SaveChanges();
                return attachment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Attachment Delete(Attachment attachment)
        {
            try
            {
                _context.Attachment.Remove(attachment);
                _context.SaveChanges();
                return attachment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<bool> IsValidAttachmentId(Guid id)
        {
            try
            {
                return await _context.Attachment.AnyAsync(a => a.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
