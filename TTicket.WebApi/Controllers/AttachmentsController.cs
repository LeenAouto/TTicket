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

        [HttpGet("GetAttachments")]
        public async Task<IActionResult> GetAll([FromQuery] AttachmentListRequestModel model)
        {
            try
            {
                var attachments = await _attachmentManager.GetList(model);
                if (!attachments.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found"},
                        ErrorCode.AttachmentsNotFound,
                        $"No attachments were found"));

                return Ok(new Response<IEnumerable<Attachment>>(attachments, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

        [HttpGet("GetAttachment")]
        public async Task<IActionResult> Get([FromQuery] AttachmentRequestModel model)
        {
            try
            {
                var attachment = await _attachmentManager.Get(model);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
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
                AttacherType attacher;
                if (await _ticketManager.Get(new TicketRequestModel { Id = dto.AttachedToId }) != null)
                    attacher = AttacherType.Ticket;
                else if (await _commentManager.Get(dto.AttachedToId) != null)
                    attacher = AttacherType.Comment;
                else
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request"},
                        ErrorCode.InvalidAttachedToId,
                        $"Invalid attachedToId"));

                if (await _attachmentManager.Get(new AttachmentRequestModel { FileName = dto.FileName}) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AttachmentFileNameAlreadyUsed,
                        $"File name is already used"));

                if (string.IsNullOrWhiteSpace(dto.FileName))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidFileName,
                        $"Attachment file name is required"));

                var attachment = new Attachment
                {
                    AttachedToId = dto.AttachedToId,
                    FileName = dto.FileName,
                    Attacher = attacher
                };

                await _attachmentManager.Add(attachment);
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
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
                var attachment = await _attachmentManager.Get(new AttachmentRequestModel { Id = id });
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                if (await _attachmentManager.Get(new AttachmentRequestModel { FileName = dto.FileName }) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AttachmentFileNameAlreadyUsed,
                        $"File name is already used"));

                if (string.IsNullOrWhiteSpace(dto.FileName))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidFileName,
                        $"Attachment file name is required"));

                attachment.FileName = dto.FileName;

                _attachmentManager.Update(attachment);
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
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
                var attachment = await _attachmentManager.Get(new AttachmentRequestModel { Id = id });
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                _attachmentManager.Delete(attachment);
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }



        //[HttpGet("GetByAttachedToId/{id}")]
        //public async Task<IActionResult> GetByAttachedToId(Guid id)
        //{
        //    try
        //    {
        //        var attachments = await _attachmentManager.GetByAttachedToId(id);
        //        if (!attachments.Any())
        //            return NotFound($"No attachments were found");

        //        return Ok(attachments);
        //    }
        //    catch (Exception e)
        //    {
        //        _logger.LogError(e, "An Error Occured In Controller.");
        //        return BadRequest(e.Message);
        //    }
        //}
    }
}
