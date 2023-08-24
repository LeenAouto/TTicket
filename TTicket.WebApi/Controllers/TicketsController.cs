using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;
using TTicket.Security.Policies;

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

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] TicketListRequestModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();
                else
                {
                    var currentUserId = HttpContext.User.FindFirstValue("uid");
                    var currentUserType = HttpContext.User.FindFirstValue("TypeUser");
                    if (currentUserType == "2")
                        model.SupportId = Guid.Parse(currentUserId);
                    else if (currentUserType == "3")
                        model.UserId = Guid.Parse(currentUserId);
                }

                var tickets = await _ticketManager.GetList(model);
                if (!tickets.Items.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketsNotFound,
                        $"No tickets were found"));

                return Ok(new Response<PagedResponse<TicketModel>>(tickets, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                var currentUserId = HttpContext.User.FindFirstValue("uid");
                var currentUserType = HttpContext.User.FindFirstValue("TypeUser");
                if (currentUserType == "2" && currentUserId != ticket.SupportId.ToString())
                    return Forbid();
                else if (currentUserType == "3" && currentUserId != ticket.UserId.ToString())
                    return Forbid();

                return Ok(new Response<TicketModel>(ticket, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ClientPolicy")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] TicketAddDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));

                //var request = new UserRequestModel { Id = dto.UserId, TypeUser = UserType.Client };
                //if (await _userManager.GetByIdentity(request) == null)
                //    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                //        ErrorCode.UserNotFound,
                //        $"No user was found"));

                if (await _productManager.Get(dto.ProductId) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductNotFound,
                        $"No product was found"));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidTicketContent,
                        $"Ticket content is required"));

                var ticket = new TicketModel
                {
                    //UserId = dto.UserId,
                    UserId = currentUserId,
                    SupportId = null,
                    ProductId = dto.ProductId,
                    Name = GenerateTicketName(),
                    Content = dto.Content,
                    CreatedDate = DateTime.Now,
                    UpdatedDate = null,
                    Status = TicketStatus.New
                };

                var result = await _ticketManager.Add(ticket);
                return Ok(new Response<TicketModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ClientPolicy")]
        [HttpPut("{id}")] 
        public async Task<IActionResult> Update(Guid id, [FromBody] TicketUpdateDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));

                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                if (ticket.UserId != currentUserId)
                    return Forbid();

                if (await _productManager.Get(dto.ProductId ?? ticket.ProductId) == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.ProductNotFound,
                        $"No product was found"));

                ticket.ProductId = dto.ProductId ?? ticket.ProductId;

                if(dto.Content != null)
                {
                    if (string.IsNullOrWhiteSpace(dto.Content))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidTicketContent,
                            $"Ticket content is required"));

                    ticket.Content = dto.Content;
                }

                ticket.UpdatedDate = DateTime.Now;

                var result = await _ticketManager.Update(ticket);
                return Ok(new Response<TicketModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [MultiplePoliciesAuthorize("ManagerPolicy;SupportPolicy")]
        [HttpPut("SetTicketStatus/{id}")]
        public async Task<IActionResult> SetTicketStatus(Guid id, [FromBody] TicketSetStatusDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                var currentUserType = HttpContext.User.FindFirstValue("TypeUser");

                if (currentUserType == "2" && ticket.SupportId == currentUserId)
                    ticket.Status = TicketStatus.Closed;
                else if (currentUserType == "1")
                {
                    var request = new UserRequestModel { Id = dto.SupportId, TypeUser = UserType.Support };
                    if (await _userManager.GetByIdentity(request) == null)
                        return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                            ErrorCode.UserNotFound,
                            $"No user was found"));

                    ticket.SupportId = dto.SupportId;
                    ticket.Status = TicketStatus.Assigned;
                }
                else
                    return Forbid();

                var result = await _ticketManager.Update(ticket);
                return Ok(new Response<TicketModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var ticket = await _ticketManager.Get(id);
                if (ticket == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                var result = await _ticketManager.Delete(ticket);
                return Ok(new Response<TicketModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [NonAction]
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
