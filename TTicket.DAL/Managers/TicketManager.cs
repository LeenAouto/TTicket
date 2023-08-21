using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.RequestModels;

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

        public async Task<Ticket> Get(Guid id)
        {
            try
            {
                return await _context.Ticket.
                    Where(t => t.Id == id).
                    SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<Ticket> GetByName(string name)
        {
            try
            {
                return await _context.Ticket.
                    Where(t => t.Name == name).
                    FirstOrDefaultAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetList(TicketListRequestModel model)
        {
            try
            {
                var skip = (model.PageNumber - 1) * model.PageSize;
                return await _context.Ticket.
                    Where(t => (t.UserId == model.UserId || model.UserId == null)
                            &&(t.SupportId == model.SupportId || model.SupportId == null)
                            &&(t.ProductId == model.SupportId || model.ProductId == null)
                            &&(t.Name == model.Name || model.Name == null)
                            &&(t.CreatedDate == model.CreatedDate || model.CreatedDate == null)
                            &&(t.UpdatedDate == model.UpdatedDate || model.UpdatedDate == null)
                            &&(t.Status == model.Status || model.Status == null)).
                    OrderByDescending(t => t.CreatedDate).
                    Skip(skip).
                    Take(model.PageSize).
                    ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<Ticket> Add(Ticket ticket)
        {
            try
            {
                await _context.Ticket.AddAsync(ticket);
                _context.SaveChanges();
                return ticket;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Ticket Update(Ticket ticket)
        {
            try
            {
                _context.Ticket.Update(ticket);
                _context.SaveChanges();
                return ticket;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public Ticket Delete(Ticket ticket)
        {
            try
            {
                _context.Ticket.Remove(ticket);
                _context.SaveChanges();
                return ticket;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
