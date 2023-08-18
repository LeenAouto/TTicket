using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models.DTOs;
using TTicket.Models;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;
using System.Xml.Linq;

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
                        ErrorCode.CommentsNotFound,
                        $"No comments were found"));

                return Ok(new Response<IEnumerable<Comment>>(comments, ErrorCode.NoError));
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
                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound,
                        $"No comment was found"));

                return Ok(new Response<Comment>(comment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] CommentAddDto dto)
        {
            try
            {
                if (await _userManager.Get(new UserRequestModel { Id = dto.UserId}) == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound,
                        $"The user was not found"));

                if (await _ticketManager.Get(new TicketRequestModel { Id = dto.TicketId }) == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.TicketNotFound,
                        $"No ticket was not found"));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidCommentContent,
                        $"Comment content is required"));

                var comment = new Comment
                {
                    TicketId = dto.TicketId,
                    UserId = dto.UserId,
                    Content = dto.Content,
                    CreatedDate = DateTime.Now
                };

                await _commentManager.Add(comment);
                return Ok(new Response<Comment>(comment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
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
                        ErrorCode.CommentsNotFound,
                        $"No comment was found"));

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidCommentContent,
                        $"Comment content is required"));

                comment.Content = dto.Content;

                _commentManager.Update(comment);
                return Ok(new Response<Comment>(comment, ErrorCode.NoError));
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
                var comment = await _commentManager.Get(id);
                if (comment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.CommentsNotFound,
                        $"No comment was found"));

                _commentManager.Delete(comment);
                return Ok(new Response<Comment>(comment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }



        //[HttpGet("GetByUserId/{id}")]
        //public async Task<IActionResult> GetByUserId(Guid id)
        //{
        //    try
        //    {
        //        if (!await _userManager.IsValidUserId(id))
        //            return NotFound($"Invalid user id");

        //        var comments = await _commentManager.GetByUserId(id);
        //        if (!comments.Any())
        //            return NotFound($"No comments were found");

        //        return Ok(comments);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "An Error Occured In Controller.");
        //        return BadRequest(e.Message);
        //    }
        //}

        //[HttpGet("GetByTicketId/{id}")]
        //public async Task<IActionResult> GetByTicketId(Guid id)
        //{
        //    try
        //    {
        //        if (!await _ticketManager.IsValidTicketId(id))
        //            return NotFound($"Invalid ticket id");

        //        var comments = await _commentManager.GetByTicketId(id);
        //        if (!comments.Any())
        //            return NotFound($"No comments were found");

        //        return Ok(comments);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "An Error Occured In Controller.");
        //        return BadRequest(e.Message);
        //    }
        //}
    }
}
