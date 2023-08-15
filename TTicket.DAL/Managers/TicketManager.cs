using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TTicket.DAL.Interfaces;
using TTicket.Models;

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
                    Include(t => t.Client).
                    Include(t => t.Support).
                    Include(t => t.Product).
                    SingleOrDefaultAsync(t => t.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetAll()
        {
            try
            {

                return await _context.Ticket.
                    OrderByDescending(t => t.CreatedDate).
                    Include(t => t.Client).
                    Include(t => t.Support).
                    Include(t => t.Product).
                    ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetByClientId(Guid id)
        {
            try
            {
                return await _context.Ticket.
                Where(t => t.ClientId == id).
                OrderByDescending(t => t.CreatedDate).
                Include(t => t.Client).
                Include(t => t.Support).
                Include(t => t.Product).
                ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetBySupportId(Guid id)
        {
            try
            {
                return await _context.Ticket.
                Where(t => t.SupportId == id).
                OrderByDescending(t => t.CreatedDate).
                Include(t => t.Client).
                Include(t => t.Support).
                Include(t => t.Product).
                ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetByProductId(Guid id)
        {
            try
            {
                return await _context.Ticket.
                Where(t => t.ProductId == id).
                OrderByDescending(t => t.CreatedDate).
                Include(t => t.Client).
                Include(t => t.Support).
                Include(t => t.Product).
                ToListAsync();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }

        public async Task<IEnumerable<Ticket>> GetByStatus(byte status)
        {
            try
            {
                return await _context.Ticket.
                Where(t => (byte)t.Status == status).
                OrderByDescending(t => t.CreatedDate).
                Include(t => t.Client).
                Include(t => t.Support).
                Include(t => t.Product).
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

        public async Task<bool> IsValidTicketId(Guid id)
        {
            try
            {
                return await _context.Ticket.AnyAsync(t => t.Id == id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured.");
                throw;
            }
        }
    }
}
