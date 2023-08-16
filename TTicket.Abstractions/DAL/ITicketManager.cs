using TTicket.Models;

namespace TTicket.Abstractions.DAL
{
    public interface ITicketManager
    {
        Task<Ticket> Get(Guid id);
        Task<IEnumerable<Ticket>> GetAll();
        Task<IEnumerable<Ticket>> GetByClientId(Guid id);
        Task<IEnumerable<Ticket>> GetBySupportId(Guid id);
        Task<IEnumerable<Ticket>> GetByProductId(Guid id);
        Task<IEnumerable<Ticket>> GetByStatus(byte status);
        Task<Ticket> Add(Ticket ticket);
        Ticket Update(Ticket ticket);
        Ticket Delete(Ticket ticket);
        Task<bool> IsValidTicketId(Guid id);
    }
}
