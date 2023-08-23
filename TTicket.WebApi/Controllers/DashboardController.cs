using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models.ResponseModels;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly ITicketManager _ticketManager;

        private readonly ILogger<DashboardController> _logger;

        public DashboardController(ITicketManager ticketManager, ILogger<DashboardController> logger)
        {
            _ticketManager = ticketManager;
            _logger = logger;
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpGet("TicketsStatus")]
        public IActionResult TicketsStatus()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var ticketsStatus = _ticketManager.TicketsStatus();

                return Ok(new Response<IQueryable>(ticketsStatus, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpGet("ProductiveEmp")]
        public IActionResult ProductiveEmp()
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var productiveEmp = _ticketManager.ProductiveEmp();

                return Ok(new Response<IQueryable>(productiveEmp, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }
    }
}
