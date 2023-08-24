using TTicket.Models.PresentationModels;
using TTicket.Models.PresentationModels.DashboardModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.Abstractions.DAL
{
    public interface ITicketManager
    {
        Task<TicketModel> Get(Guid id);
        Task<PagedResponse<TicketModel>> GetList(TicketListRequestModel model);
        Task<TicketModel> Add(TicketModel ticket);
        Task<TicketModel> Update(TicketModel ticket);
        Task<TicketModel> Delete(TicketModel ticket);

        Task<IEnumerable<CountersModel>> TicketsStatus();
        Task<IEnumerable<ProductiveEmpModel>> ProductiveEmp();
        Task<IEnumerable<ProductsTicketsModel>> ProductsTicketsCounter();
    }
}
