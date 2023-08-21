using TTicket.Models;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface ITicketManager
    {
        Task<Ticket> Get(Guid id);
        Task<Ticket> GetByName(string name);
        Task<IEnumerable<Ticket>> GetList(TicketListRequestModel model);
        Task<Ticket> Add(Ticket ticket);
        Ticket Update(Ticket ticket);
        Ticket Delete(Ticket ticket);
    }
}
