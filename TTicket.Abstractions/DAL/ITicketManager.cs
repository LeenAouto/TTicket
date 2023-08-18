using TTicket.Models;
using TTicket.Models.RequestModels;

namespace TTicket.Abstractions.DAL
{
    public interface ITicketManager
    {
        Task<Ticket> Get(TicketRequestModel model);
        Task<IEnumerable<Ticket>> GetList(TicketListRequestModel model);
        Task<Ticket> Add(Ticket ticket);
        Ticket Update(Ticket ticket);
        Ticket Delete(Ticket ticket);



        //Task<IEnumerable<Ticket>> GetByClientId(Guid id);
        //Task<IEnumerable<Ticket>> GetBySupportId(Guid id);
        //Task<IEnumerable<Ticket>> GetByProductId(Guid id);
        //Task<IEnumerable<Ticket>> GetByStatus(byte status);
        //Task<bool> IsValidTicketId(Guid id);
    }
}
