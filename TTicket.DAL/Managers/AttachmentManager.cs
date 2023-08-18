using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.RequestModels;

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

        public async Task<Attachment> Get(AttachmentRequestModel model)
        {
            try
            {
                return await _context.Attachment.
                    Where(a => a.Id == model.Id || a.FileName == model.FileName).
                    FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Attachment>> GetList(AttachmentListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;
                return await _context.Attachment.
                    Where(a => (a.AttachedToId == model.AttachedToId || model.AttachedToId == null)
                            && (a.FileName == model.FileName || model.FileName == null)
                            && (a.Attacher == model.Attacher || model.Attacher == null)
                            ).
                    Skip(skip).
                    Take(model.PageSize).
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

        //public async Task<IEnumerable<Attachment>> GetByAttachedToId(Guid id)
        //{
        //    try
        //    {
        //        return await _context.Attachment.
        //            Where(a => a.AttachedToId == id).
        //            OrderBy(a => a.Id).
        //            ToListAsync();
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "An Error Occured.");
        //        throw;
        //    }
        //}


        //public async Task<bool> IsValidAttachmentId(Guid id)
        //{
        //    try
        //    {
        //        return await _context.Attachment.AnyAsync(a => a.Id == id);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "An Error Occured.");
        //        throw;
        //    }
        //}
    }
}
