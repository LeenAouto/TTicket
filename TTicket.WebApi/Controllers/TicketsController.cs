using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketManager _ticketManager;
        private readonly IUserManager _userManager;
        private readonly IProductManager _productManager;

        private readonly ILogger<TicketsController> _logger;

        public TicketsController(ITicketManager ticketManager, ILogger<TicketsController> logger, IUserManager userManager, IProductManager productManager)
        {
            _ticketManager = ticketManager;
            _logger = logger;
            _userManager = userManager;
            _productManager = productManager;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TicketListRequestModel model)
        {
            try
            {
                var tickets = await _ticketManager.GetList(model);
                if (!tickets.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketsNotFound,
                        $"No tickets were found"));

                return Ok(new Response<IEnumerable<Ticket>>(tickets, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TicketAddDto dto)
        {
            try
            {
                var request = new UserRequestModel { Id = dto.UserId, TypeUser = UserType.Client };
                if (await _userManager.GetByIdentity(request) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UserNotFound,
                        $"No user was found"));

                if (await _productManager.Get(dto.ProductId) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductNotFound,
                        $"No product was found"));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidTicketContent,
                        $"Ticket content is required"));

                var ticket = new Ticket
                {
                    UserId = dto.UserId,
                    SupportId = null,
                    ProductId = dto.ProductId,
                    Name = GenerateTicketName(),
                    Content = dto.Content,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    Status = TicketStatus.New
                };

                await _ticketManager.Add(ticket);
                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPut("{id}")] //Used by client users
        public async Task<IActionResult> Update(Guid id, [FromBody] TicketUpdateDto dto)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                if (await _productManager.Get(dto.ProductId) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductNotFound,
                        $"No product was found"));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidTicketContent,
                        $"Ticket content is required"));

                ticket.ProductId = dto.ProductId;
                ticket.Content = dto.Content;
                ticket.UpdatedDate = DateTime.Now;

                _ticketManager.Update(ticket);
                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPut("assign/{id}")] //Used by manager
        public async Task<IActionResult> Assign(Guid id, [FromBody] TicketAssignDto dto)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                var request = new UserRequestModel { Id = dto.SupportId, TypeUser = UserType.Support };
                if (await _userManager.GetByIdentity(request) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UserNotFound,
                        $"No user was found"));

                if (ticket.Status == TicketStatus.Assigned)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.TicketAlreadyAssigned,
                        $"The ticket is already assigned to support member with id = {ticket.SupportId}"));

                if (ticket.Status == TicketStatus.Closed)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.TicketAlreadyClosed,
                        $"The ticket is already closed"));

                ticket.SupportId = dto.SupportId;
                ticket.Status = TicketStatus.Assigned;

                _ticketManager.Update(ticket);
                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPut("reassign/{id}")] //Used by manager
        public async Task<IActionResult> Reassign(Guid id, [FromBody] TicketAssignDto dto)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                var request = new UserRequestModel { Id = dto.SupportId, TypeUser = UserType.Support };
                if (await _userManager.GetByIdentity(request) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UserNotFound,
                        $"No user was found"));

                ticket.SupportId = dto.SupportId;
                ticket.Status = TicketStatus.Assigned;

                _ticketManager.Update(ticket);
                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPut("close/{id}")] //Used by support member
        public async Task<IActionResult> Close(Guid id)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                if ((byte)ticket.Status == 3)
                    if (ticket.Status == TicketStatus.Closed)
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.TicketAlreadyClosed,
                            $"The ticket is already closed"));

                ticket.Status = TicketStatus.Closed;

                _ticketManager.Update(ticket);
                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                _ticketManager.Delete(ticket);
                return Ok(new Response<Ticket>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        private static string GenerateTicketName()
        {
            DateTime currentDate = DateTime.Now;
            string year = currentDate.Year.ToString();
            string month = currentDate.Month.ToString("D2");
            string day = currentDate.Day.ToString("D2");
            string randomPart = new Random().Next(100000).ToString();

            string ticketName = $"Ticket#{year}{month}{day}{randomPart}";
            return ticketName;
        }
    }
}
