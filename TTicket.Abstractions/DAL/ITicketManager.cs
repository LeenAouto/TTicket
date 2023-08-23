﻿using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.Abstractions.DAL
{
    public interface ITicketManager
    {
        Task<TicketModel> Get(Guid id);
        //Task<TicketModel> GetByName(string name);
        Task<PagedResponse<TicketModel>> GetList(TicketListRequestModel model);
        Task<TicketModel> Add(TicketModel ticket);
        Task<TicketModel> Update(TicketModel ticket);
        Task<TicketModel> Delete(TicketModel ticket);

        IQueryable TicketsStatus();
        IQueryable ProductiveEmp();
    }
}
