using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models.DTOs;
using TTicket.Models;

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

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var comments = await _commentManager.GetAll();
                if (!comments.Any())
                    return NotFound($"No comments were found");

                return Ok(comments);
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
                    return NotFound($"No comment with id = {id} was found");

                return Ok(comment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByUserId/{id}")]
        public async Task<IActionResult> GetByUserId(Guid id)
        {
            try
            {
                if (!await _userManager.IsValidUserId(id))
                    return NotFound($"Invalid user id");

                var comments = await _commentManager.GetByUserId(id);
                if (!comments.Any())
                    return NotFound($"No comments were found");

                return Ok(comments);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByTicketId/{id}")]
        public async Task<IActionResult> GetByTicketId(Guid id)
        {
            try
            {
                if (!await _ticketManager.IsValidTicketId(id))
                    return NotFound($"Invalid ticket id");

                var comments = await _commentManager.GetByTicketId(id);
                if (!comments.Any())
                    return NotFound($"No comments were found");

                return Ok(comments);
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
                if (!await _userManager.IsValidUserId(dto.UserId))
                    return BadRequest($"Invalid user id");
                if (!await _ticketManager.IsValidTicketId(dto.TicketId))
                    return BadRequest($"Invalid ticket id");

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest($"Comment content is required");

                var comment = new Comment
                {
                    TicketId = dto.TicketId,
                    UserId = dto.UserId,
                    Content = dto.Content,
                    CreatedDate = DateTime.Now
                };

                await _commentManager.Add(comment);
                return Ok(comment);
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
                    return NotFound($"No comment with id = {id} was found");

                if (string.IsNullOrWhiteSpace(dto.Content))
                    return BadRequest($"Comment content is required");

                comment.Content = dto.Content;

                _commentManager.Update(comment);
                return Ok(comment);
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
                    return NotFound($"No comment with id = {id} was found.");

                _commentManager.Delete(comment);
                return Ok(comment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }
    }
}
