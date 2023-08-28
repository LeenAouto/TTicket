using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using TTicket.Abstractions.DAL;
using TTicket.Models;
using TTicket.Models.DTOs;
using TTicket.Models.PresentationModels;
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

        /// <summary>
        /// Get all the attachments detalis of a specific ticket or a comment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetAttachments")]
        public async Task<IActionResult> GetAll([FromQuery] AttachmentListRequestModel model)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                if (model.AttachedToId == Guid.Empty)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.RequiredFilter,
                        "AttachedTo is Required"));

                var attachments = await _attachmentManager.GetList(model);
                if (!attachments.Items.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found"},
                        ErrorCode.AttachmentsNotFound,
                        $"No attachments were found"));

                return Ok(new Response<PagedResponse<AttachmentModel>>(attachments, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        /// <summary>
        /// Get the details about a single attachment using the attachment id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetInfo/{id}")]
        public async Task<IActionResult> GetInfo(Guid id)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                return Ok(new Response<AttachmentModel>(attachment, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        /// <summary>
        /// Download an attachment by its id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("DownloadAttachmentById/{id}")]
        public async Task<IActionResult> DownloadAttachmentById(Guid id)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                var targetPath = Path.Combine(GetFilesPath(attachment.AttachedToId), attachment.FileName);
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


        /// <summary>
        /// Download an attachment by its file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        //[Authorize]
        [HttpGet("DownloadAttachmentByName/{fileName}")]
        public async Task<IActionResult> DownloadAttachmentByName(string fileName)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var attachment = await _attachmentManager.GetByFileName(fileName);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                var targetPath = Path.Combine(GetFilesPath(attachment.AttachedToId), attachment.FileName);
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(targetPath, out var contentType))
                    contentType = "application/octet-stream";
               
                HttpContext.Response.ContentType = contentType;

                return PhysicalFile(targetPath, contentType);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }


        /// <summary>
        /// Upload (add) attachment to a ticket of a comment
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize] 
        [HttpPost("UploadAttachment")]
        public async Task<IActionResult> UploadAttachment([FromForm] AttachmentAddDto dto)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                AttacherType attacher;
                if (await _ticketManager.Get(dto.AttachedToId) != null)
                    attacher = AttacherType.Ticket;
                else if (await _commentManager.Get(dto.AttachedToId) != null)
                    attacher = AttacherType.Comment;
                else
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request"},
                        ErrorCode.InvalidAttachedToId,
                        $"Invalid attachedToId"));

                if (dto.AttachmentFile.Length <= 0)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AttachmentRequired,
                        $"Attachment file is required"));

                var filesPath = GetFilesPath(dto.AttachedToId);

                if(!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var fileName = GenerateFileName(dto.AttachmentFile);

                var targetPath = Path.Combine(filesPath, fileName);
                using(var stream = new FileStream(targetPath, FileMode.Create))
                {
                    await dto.AttachmentFile.CopyToAsync(stream);
                }

                var attachment = new AttachmentModel
                {
                    AttachedToId = dto.AttachedToId,
                    FileName = fileName,
                    Attacher = attacher
                };

                var result = await _attachmentManager.Add(attachment);
                return Ok(new Response<AttachmentModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        /// <summary>
        /// Update an attachment (The old one gets deleted from the server)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromForm] AttachmentUpdateDto dto)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

                var attachment = await _attachmentManager.Get(id);
                if (attachment == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.AttachmentNotFound,
                        $"No attachment was found"));

                if(dto.AttachmentFile == null || dto.AttachmentFile.Length <= 0)
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

                var result = await _attachmentManager.Update(attachment);
                return Ok(new Response<AttachmentModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        /// <summary>
        /// Delete an attachment from the server
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

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

                var result = await _attachmentManager.Delete(attachment);
                return Ok(new Response<AttachmentModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [NonAction]
        private string GetFilesPath(Guid id)
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build().GetSection("Path").GetSection("AttachmentUploadPath").Value + id.ToString();
        }

        [NonAction]
        private string GenerateFileName(IFormFile file)
        {
            var extension = "." + file.FileName.Split('.').Last();
            var fileName = DateTime.Now.Ticks.ToString() + extension;
            return fileName;
        }

    }
}
