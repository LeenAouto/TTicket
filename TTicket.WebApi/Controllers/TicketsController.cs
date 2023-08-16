using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.DAL.Managers;
using TTicket.Models;
using TTicket.Models.DTOs;

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
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var tickets = await _ticketManager.GetAll();
                if (!tickets.Any())
                    return NotFound($"No tickets were found");

                return Ok(tickets);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound($"No ticket with id = {id} was found");

                return Ok(ticket);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByClientId/{id}")]
        public async Task<IActionResult> GetByClientId(Guid id)
        {
            try
            {
                if (!await _userManager.IsValidUserId(id))
                    return NotFound($"Invalid user id");
                if (!await _userManager.IsClient(id))
                    return NotFound($"The user is not a client");

                var clientTickets = await _ticketManager.GetByClientId(id);
                if (!clientTickets.Any())
                    return NotFound($"No tickets were found");

                return Ok(clientTickets);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetBySupportId/{id}")]
        public async Task<IActionResult> GetBySupportId(Guid id)
        {
            try
            {
                if (!await _userManager.IsValidUserId(id))
                    return NotFound($"Invalid user id");
                if (!await _userManager.IsSupport(id))
                    return NotFound($"The user is not a support member");

                var supportTickets = await _ticketManager.GetBySupportId(id);
                if (!supportTickets.Any())
                    return NotFound($"No tickets were found");

                return Ok(supportTickets);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByProductId/{id}")]
        public async Task<IActionResult> GetByProductId(Guid id)
        {
            try
            {
                if (!await _productManager.IsValidProductId(id))
                    return NotFound($"Invalid product id");

                var productTickets = await _ticketManager.GetByProductId(id);
                if (!productTickets.Any())
                    return NotFound($"No tickets were found");

                return Ok(productTickets);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByStatus/{status}")]
        public async Task<IActionResult> GetByStatus(byte status)
        {
            try
            {
                if (!(status >= 1 && status <= 3))
                    return NotFound($"Invalid status");

                var statusTickets = await _ticketManager.GetByStatus(status);
                if (!statusTickets.Any())
                    return NotFound($"No tickets were found");

                return Ok(statusTickets);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TicketAddDto dto)
        {
            try
            {
                if (!await _userManager.IsClient(dto.ClientId))
                    return BadRequest($"Invalid client user id");
                if (!await _productManager.IsValidProductId(dto.ProductId))
                    return BadRequest($"Invalid product id");

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest($"Ticket content is required");

                var ticket = new Ticket
                {
                    ClientId = dto.ClientId,
                    SupportId = null,
                    ProductId = dto.ProductId,
                    Name = GenerateTicketName(),
                    Content = dto.Content,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    Status = TicketStatus.New
                };

                await _ticketManager.Add(ticket);
                return Ok(ticket);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")] //Used by client users
        public async Task<IActionResult> Update(Guid id, [FromBody] TicketUpdateDto dto)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound($"No ticket with id = {id} was found");

                if (!await _productManager.IsValidProductId(dto.ProductId))
                    return BadRequest($"Invalid product id");

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest($"Ticket content is required");

                ticket.ProductId = dto.ProductId;
                ticket.Content = dto.Content;
                ticket.UpdatedDate = DateTime.Now;

                _ticketManager.Update(ticket);
                return Ok(ticket);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("assign/{id}")] //Used by support members
        public async Task<IActionResult> Assign(Guid id, [FromBody] TicketAssignDto dto)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound($"No ticket with id = {id} was found");

                if (!await _userManager.IsValidUserId(dto.SupportId))
                    return BadRequest($"Invalid user id");
                if (!await _userManager.IsSupport(dto.SupportId))
                    return BadRequest($"The user is not a support member");

                ticket.SupportId = dto.SupportId;
                ticket.Status = TicketStatus.Assigned;

                _ticketManager.Update(ticket);
                return Ok(ticket);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound($"No ticket with id = {id} was found");

                _ticketManager.Delete(ticket);
                return Ok(ticket);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
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
