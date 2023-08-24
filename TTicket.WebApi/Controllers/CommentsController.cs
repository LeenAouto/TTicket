using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models.DTOs;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;
using TTicket.Models.PresentationModels;
using Microsoft.AspNetCore.Authorization;
using TTicket.Security.Policies;
using System.Security.Claims;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentManager _commentManager;
        private readonly ITicketManager _ticketManager;

        private readonly ILogger<CommentsController> _logger;

        public CommentsController(ICommentManager commentManager, ITicketManager ticketManager, ILogger<CommentsController> logger)
        {
            _commentManager = commentManager;
            _ticketManager = ticketManager;
            _logger = logger;
        }

        /// <summary>
        /// Get the list of comments that are added to a specific ticket
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetComments")]
        public async Task<IActionResult> GetAll([FromQuery] CommentListRequestModel model)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                if(model.TicketId == Guid.Empty)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request"},
                        ErrorCode.RequiredFilter,
                        "TicketId is Required"));


                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");

                if(CurrentUserType != "1")
                {
                    var ticket = await _ticketManager.Get(model.TicketId);
                    if (ticket == null)
                        return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                    if (currentUserId != ticket.UserId && currentUserId != ticket.SupportId)
                        return Forbid();
                }

                var comments = await _commentManager.GetList(model);
                if (!comments.Items.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comments were found"));

                return Ok(new Response<PagedResponse<CommentModel>>(comments, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        /// <summary>
        /// Get the details of a specific comment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comment was found"));

                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");

                if (CurrentUserType != "1")
                {
                    var ticket = await _ticketManager.Get(comment.TicketId);
                    if (ticket == null)
                        return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was found"));

                    if (currentUserId != ticket.UserId && currentUserId != ticket.SupportId)
                        return Forbid();
                }

                return Ok(new Response<CommentModel>(comment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }


        /// <summary>
        /// Add a comment to a ticket 
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [MultiplePoliciesAuthorize("SupportPolicy;ClientPolicy")]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CommentAddDto dto)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));

                //var user = await _userManager.Get(dto.UserId);
                //if (user == null)
                //    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                //        ErrorCode.UserNotFound, $"The user was not found"));

                var ticket = await _ticketManager.Get(dto.TicketId);
                if (ticket == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.TicketNotFound, $"No ticket was not found"));

                if (ticket.UserId != currentUserId && ticket.SupportId != currentUserId)
                    return Forbid();
                    //return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                    //    ErrorCode.ForbidAccess, $"Only the client that created the ticket and the support employee can comment"));


                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidCommentContent, $"Comment content is required"));

                var comment = new CommentModel
                {
                    TicketId = dto.TicketId,
                    UserId = currentUserId,
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

        /// <summary>
        /// Update a comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [MultiplePoliciesAuthorize("SupportPolicy;ClientPolicy")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] CommentUpdateDto dto)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));

                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound, $"No comment was found"));

                if (comment.UserId != currentUserId)
                    return Forbid();

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

        /// <summary>
        /// Delete a comment (Only the manager is allowed to delete a comment)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Policy = "ManagerPolicy")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

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
