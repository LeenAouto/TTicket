using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AttachmentsController : ControllerBase
    {
        private readonly IAttachmentManager _attachmentManager;
        private readonly ITicketManager _ticketManager;
        private readonly ICommentManager _commentManager;

        private readonly ILogger<AttachmentsController> _logger;

        public AttachmentsController(IAttachmentManager attachmentManager, ITicketManager ticketManager, ICommentManager commentManager, ILogger<AttachmentsController> logger)
        {
            _attachmentManager = attachmentManager;
            _ticketManager = ticketManager;
            _commentManager = commentManager;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var attachments = await _attachmentManager.GetAll();
                if (!attachments.Any())
                    return NotFound($"No attachments were found");

                return Ok(attachments);
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
                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound($"No attachment with id = {id} was found");

                return Ok(attachment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetByAttachedToId/{id}")]
        public async Task<IActionResult> GetByAttachedToId(Guid id)
        {
            try
            {
                var attachments = await _attachmentManager.GetByAttachedToId(id);
                if (!attachments.Any())
                    return NotFound($"No attachments were found");

                return Ok(attachments);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] AttachmentAddDto dto)
        {
            try
            {
                if (!(await _ticketManager.IsValidTicketId(dto.AttachedToId) || await _commentManager.IsValidCommentId(dto.AttachedToId)))
                    return BadRequest($"Invalid attachedTo id");

                if (!(dto.Attacher >= 1 && dto.Attacher <= 2))
                    return BadRequest($"Invalid attacher type");

                if (string.IsNullOrWhiteSpace(dto.FileName))
                    return BadRequest($"Attachment file name is required");

                var attachment = new Attachment
                {
                    AttachedToId = dto.AttachedToId,
                    FileName = dto.FileName,
                    Attacher = dto.Attacher == 1? AttacherType.Ticket : AttacherType.Comment
                };

                await _attachmentManager.Add(attachment);
                return Ok(attachment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] AttachmentUpdateDto dto)
        {
            try
            {
                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound($"No attachment with id = {id} was found");

                if(string.IsNullOrWhiteSpace(dto.FileName))
                    return BadRequest($"Attachment file name is required");

                attachment.FileName = dto.FileName;

                _attachmentManager.Update(attachment);
                return Ok(attachment);
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
                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound($"No attachment with id = {id} was found");

                _attachmentManager.Delete(attachment);
                return Ok(attachment);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }
    }
}
