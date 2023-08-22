using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models.DTOs;
using TTicket.Models;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;
using TTicket.Models.PresentationModels;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentManager _commentManager;
        private readonly IUserManager _userManager;
        private readonly ITicketManager _ticketManager;

        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentManager commentManager, ITicketManager ticketManager, IUserManager userManager, ILogger<CommentsController> logger)
        {
            _commentManager = commentManager;
            _ticketManager = ticketManager;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpGet("GetComments")]
        public async Task<IActionResult> GetAll([FromQuery] CommentListRequestModel model)
        {
            try
            {
                var comments = await _commentManager.GetList(model);
                if (!comments.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comments were found"));

                return Ok(new Response<IEnumerable<CommentModel>>(comments, ErrorCode.NoError));
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
                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comment was found"));

                return Ok(new Response<CommentModel>(comment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CommentAddDto dto)
        {
            try
            {
                var user = await _userManager.Get(dto.UserId);
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound, $"The user was not found"));

                var ticket = await _ticketManager.Get(dto.TicketId);
                if (ticket == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.TicketNotFound, $"No ticket was not found"));
                
                if(ticket.UserId != user.Id || ticket.SupportId != user.Id)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.ForbidAccess, $"Only the client that created the ticket and the support employee can comment"));


                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidCommentContent, $"Comment content is required"));

                var comment = new CommentModel
                {
                    TicketId = dto.TicketId,
                    UserId = dto.UserId,
                    Content = dto.Content,
                    CreatedDate = DateTime.Now
                };

                var result = await _commentManager.Add(comment);
                return Ok(new Response<CommentModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CommentUpdateDto dto)
        {
            try
            {
                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comment was found"));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidCommentContent, $"Comment content is required"));

                comment.Content = dto.Content;

                var result = await _commentManager.Update(comment);
                return Ok(new Response<CommentModel>(result, ErrorCode.NoError));
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
                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comment was found"));

                var result = await _commentManager.Delete(comment);
                return Ok(new Response<CommentModel>(result, ErrorCode.NoError));
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
