using Azure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

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

        public async Task<AttachmentModel> Get(Guid id)
        {
            try
            {
                var attachment = await _context.Attachment.
                    Where(a => a.Id == id).
                    Select(a => new AttachmentModel(a)).
                    SingleOrDefaultAsync();

                return attachment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<AttachmentModel> GetByName(string fileName)
        {
            try
            {
                var attachment = await _context.Attachment.
                    Where(a => a.FileName == fileName).
                    Select(a => new AttachmentModel(a)).
                    FirstOrDefaultAsync();

                return attachment;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<PagedResponse<AttachmentModel>> GetList(AttachmentListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;

                var query = _context.Attachment.
                    Where(a => a.AttachedToId == model.AttachedToId || model.AttachedToId == null);

                var totalCount = await query.CountAsync();

                var attachments = await query.
                    OrderBy(a => a.Id).
                    Skip(skip).
                    Take(model.PageSize).
                    Select(a => new AttachmentModel(a)).
                    ToListAsync();

                return new PagedResponse<AttachmentModel>(attachments, totalCount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<AttachmentModel> Add(AttachmentModel attachment)
        {
            try
            {
                var a = new Attachment
                {
                    AttachedToId = attachment.AttachedToId,
                    FileName = attachment.FileName,
                    Attacher = attachment.Attacher
                };

                await _context.Attachment.AddAsync(a);
                _context.SaveChanges();

                return new AttachmentModel(a);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<AttachmentModel> Update(AttachmentModel attachment)
        {
            try
            {
                var a = await _context.Attachment.Where(a => a.Id == attachment.Id).SingleAsync();

                a.AttachedToId = attachment.AttachedToId;
                a.FileName = attachment.FileName;
                a.Attacher = attachment.Attacher;

                _context.Attachment.Update(a);
                _context.SaveChanges();

                return new AttachmentModel(a);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<AttachmentModel> Delete(AttachmentModel attachment)
        {
            try
            {
                var a = await _context.Attachment.Where(a => a.Id == attachment.Id).SingleAsync();

                _context.Attachment.Remove(a);
                _context.SaveChanges();

                return new AttachmentModel(a);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
