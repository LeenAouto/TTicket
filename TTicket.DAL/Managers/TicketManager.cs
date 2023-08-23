using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.DAL.Managers
{
    public class TicketManager : ITicketManager
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TicketManager> _logger;

        public TicketManager(ILogger<TicketManager> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<TicketModel> Get(Guid id)
        {
            try
            {
                var ticket = await _context.Ticket.
                    Where(t => t.Id == id).
                    Select(t => new TicketModel(t)).
                    SingleOrDefaultAsync();

                if(ticket != null)
                {
                    var attachments = await _context.Attachment.
                        Where(a => a.AttachedToId == ticket.Id).
                        Select(a => new AttachmentModel(a)).
                        ToListAsync();

                    ticket.AttachmentsList = attachments;
                }

                return ticket;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        //public async Task<TicketModel> GetByName(string name)
        //{
        //    try
        //    {
        //        var ticket = await _context.Ticket.Where(t => t.Name == name).FirstOrDefaultAsync();

        //        return ticket != null ? new TicketModel(ticket) : null;
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "An Error Occured.");
        //        throw;
        //    }
        //}

        public async Task<PagedResponse<TicketModel>> GetList(TicketListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;

                var query = _context.Ticket.
                    Where(t => (t.UserId == model.UserId || model.UserId == null)
                            && (t.SupportId == model.SupportId || model.SupportId == null)
                            && (t.ProductId == model.SupportId || model.ProductId == null)
                            && (t.Name == model.Name || model.Name == null)
                            && (t.CreatedDate == model.CreatedDate || model.CreatedDate == null)
                            && (t.UpdatedDate == model.UpdatedDate || model.UpdatedDate == null)
                            && (t.Status == model.Status || model.Status == null));

                var totalCount = await query.CountAsync();

                var tickets =  await query.
                    OrderByDescending(t => t.CreatedDate).
                    Skip(skip).
                    Take(model.PageSize).
                    Select(t => new TicketModel(t)).
                    ToListAsync();

                return new PagedResponse<TicketModel>(tickets, totalCount);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<TicketModel> Add(TicketModel ticket)
        {
            try
            {
                var t = new Ticket
                {
                    UserId = ticket.UserId,
                    SupportId = ticket.SupportId,
                    ProductId = ticket.ProductId,
                    Name = ticket.Name,
                    Content = ticket.Content,
                    CreatedDate = ticket.CreatedDate,
                    UpdatedDate = ticket.UpdatedDate,
                    Status = ticket.Status
                };

                await _context.Ticket.AddAsync(t);
                _context.SaveChanges();

                return new TicketModel(t);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<TicketModel> Update(TicketModel ticket)
        {
            try
            {
                var t = await _context.Ticket.Where(t => t.Id == ticket.Id).SingleAsync();

                t.Id = ticket.Id;
                t.UserId = ticket.UserId;
                t.SupportId = ticket.SupportId;
                t.ProductId = ticket.ProductId;
                t.Name = ticket.Name;
                t.Content = ticket.Content;
                t.CreatedDate = ticket.CreatedDate;
                t.UpdatedDate = ticket.UpdatedDate;
                t.Status = ticket.Status;

                _context.Ticket.Update(t);
                _context.SaveChanges();

                return new TicketModel(t);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<TicketModel> Delete(TicketModel ticket)
        {
            try
            {
                var t = await _context.Ticket.Where(t => t.Id == ticket.Id).SingleAsync();

                _context.Ticket.Remove(t);
                _context.SaveChanges();

                return new TicketModel(t);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }


        public IQueryable TicketsStatus()
        {
            try
            {
                var x = _context.Ticket
                   .GroupBy(t => t.Status)
                   .Select(g => new
                   {
                       Status = g.Key.ToString(),
                       Count = g.Count()
                   });

                return x;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public IQueryable ProductiveEmp()
        {
            try
            {
                var x = _context.Ticket
                   .GroupBy(t => t.SupportId)
                   .Select(g => new
                   {
                       Employee = g.Key.ToString(),
                       ClosedTickets = g.Count(t => t.Status == TicketStatus.Closed)
                   })
                   .OrderByDescending(g => g.ClosedTickets);

                return x;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
