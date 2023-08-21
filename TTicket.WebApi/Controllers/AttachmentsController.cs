using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
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

        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<AttachmentsController> _logger;

        public AttachmentsController(IAttachmentManager attachmentManager, ITicketManager ticketManager, ICommentManager commentManager, ILogger<AttachmentsController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _attachmentManager = attachmentManager;
            _ticketManager = ticketManager;
            _commentManager = commentManager;

            _webHostEnvironment = webHostEnvironment;

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
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        //getinfo 
        [HttpGet("GetInfo/{id}")]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            try
            {
                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));
                //File.Read.
                //return File().
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        //get download
        [HttpGet("DownloadAttachment")]
        public async Task<IActionResult> DownloadAttachment([FromQuery] AttachmentDownloadDto dto)
        {
            try
            {
                var targetPath = Path.Combine(GetFilesPath(dto.AttachedToId), dto.FileName);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(targetPath, out var contentType))
                    contentType = "application/octet-stream";

                var bytes = await System.IO.File.ReadAllBytesAsync(targetPath);
                return File(bytes, contentType, Path.GetFileName(targetPath));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        //Upload 
        [HttpPost("UploadAttachment")]
        public async Task<IActionResult> UploadAttachment([FromForm] AttachmentAddDto dto)
        {
            try
            {
                AttacherType attacher;
                if (await _ticketManager.Get(dto.AttachedToId) != null)
                    attacher = AttacherType.Ticket;
                else if (await _commentManager.Get(dto.AttachedToId) != null)
                    attacher = AttacherType.Comment;
                else
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request"},
                        ErrorCode.InvalidAttachedToId,
                        $"Invalid attachedToId"));

                /*
                if (await _attachmentManager.GetByName(dto.FileName) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AttachmentFileNameAlreadyUsed,
                        $"File name is already used"));

                
                if (string.IsNullOrWhiteSpace(dto.FileName))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidFileName,
                        $"Attachment file name is required"));
                */

                var filesPath = GetFilesPath(dto.AttachedToId);

                if(!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var fileName = GenerateFileName(dto.AttachmentFile);

                var targetPath = Path.Combine(filesPath, fileName);
                using(var stream = new FileStream(targetPath, FileMode.Create))
                {
                    await dto.AttachmentFile.CopyToAsync(stream);
                }

                var attachment = new Attachment
                {
                    AttachedToId = dto.AttachedToId,
                    FileName = fileName,
                    Attacher = attacher
                };

                await _attachmentManager.Add(attachment);
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] AttachmentUpdateDto dto)
        {
            try
            {
                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));
                /*
                if (await _attachmentManager.GetByName(dto.FileName) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AttachmentFileNameAlreadyUsed,
                        $"File name is already used"));

                if (string.IsNullOrWhiteSpace(dto.FileName))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidFileName,
                        $"Attachment file name is required"));
                */
                if(dto.AttachmentFile.Length <= 0)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AttachmentRequired,
                        $"Attachment file is required"));

                var filesPath = GetFilesPath(attachment.AttachedToId);

                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var fileName = attachment.FileName;

                var targetPath = Path.Combine(filesPath, fileName);
                using (var stream = new FileStream(targetPath, FileMode.Create))
                {
                    await dto.AttachmentFile.CopyToAsync(stream);
                }

                attachment.FileName = fileName;

                _attachmentManager.Update(attachment);
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
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
                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                var filesPath = GetFilesPath(attachment.AttachedToId);

                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var fileName = attachment.FileName;

                var targetPath = Path.Combine(filesPath, fileName);

                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);
                }

                _attachmentManager.Delete(attachment);
                return Ok(new Response<Attachment>(attachment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        private string GetFilesPath(Guid id)
        {
            return _webHostEnvironment.WebRootPath + "\\Uploads\\Attachments\\" + id.ToString();
        }

        private string GenerateFileName(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.').Last();
            var fileName = DateTime.Now.Ticks.ToString() + extension;
            return fileName;
        }

    }
}
