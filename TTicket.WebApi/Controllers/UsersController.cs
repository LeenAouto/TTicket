using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;
using TTicket.Abstractions.DAL;
using TTicket.Abstractions.Security;
using TTicket.DAL.Managers;
using TTicket.Models;
using TTicket.Models.DTOs;
using TTicket.Models.PresentationModels;
using TTicket.Models.RequestModels;
using TTicket.Models.ResponseModels;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserManager _userManager;
        private readonly IAuthManager _authManager;

        private readonly IPasswordHasher _hasher;

        private readonly ILogger<UsersController> _logger;


        public UsersController(IUserManager userManager, ILogger<UsersController> logger, IPasswordHasher hasher, IAuthManager authManager)
        {
            _userManager = userManager;
            _authManager = authManager;
            _hasher = hasher;
            _logger = logger;
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetAll([FromQuery] UserListRequestModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var users = await _userManager.GetList(model);
                if (!users.Items.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UsersNotFound,
                        $"No users were found"));

                return Ok(new Response<PagedResponse<UserModel>>(users, ErrorCode.NoError));
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
                else
                {
                    var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                    var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");
                    if (currentUserId != id && CurrentUserType != "1")
                        return Forbid();
                }

                var user = await _userManager.Get(id);
                if(user == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" }, 
                        ErrorCode.UserNotFound, 
                        $"The user was not found"));

            return Ok(new Response<UserModel>(user, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize]
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();
                else
                {
                    var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                    var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");
                    if (currentUserId != id && CurrentUserType != "1")
                        return Forbid();
                }

                var user = await _userManager.Get(id);
                if (user == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UserNotFound,
                        $"No user was found"));

                var targetPath = GetUserImage(user.Id);
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



        [Authorize(Policy = "ManagerPolicy")]
        [HttpPut("{id}")] //used by manager
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UserUpdateDto dto)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var user = await _userManager.GetByIdentity(new UserRequestModel { Id = id });
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request"}, 
                        ErrorCode.UserNotFound, 
                        $"Unable to update the user becuase it does not exist"));

                //Validate username
                if(dto.Username != null)
                {
                    if (!_authManager.IsValidUserName(dto.Username))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidUsername,
                            $"Usernames can only be of English characters"));
                    else if (dto.Username != user.Username && await _userManager.GetByIdentity(new UserRequestModel { Identity = dto.Username }) != null)
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.UsernameAlreadyUsed,
                            $"This username is already used by another account"));

                    user.Username = dto.Username;
                }

                //Validate Email
                if(dto.Email != null)
                {
                    if (!_authManager.IsValidEmailAddress(dto.Email))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidEmailAddress,
                            $"Invalid email address"));
                    else if (dto.Email != user.Email && await _userManager.GetByIdentity(new UserRequestModel { Identity = dto.Email }) != null)
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.UserEmailAlreadyUsed,
                            $"This email is already used by another account"));

                    user.Email = dto.Email;
                }

                //Validate mobile number
                if(dto.MobilePhone != null)
                {
                    if (!_authManager.IsValidMobileNumber(dto.MobilePhone))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidPhoneNumber,
                            $"Phone number must start with 05 and be exactly 10 digits"));
                    else if (dto.MobilePhone != user.MobilePhone && await _userManager.GetByIdentity(new UserRequestModel { Identity = dto.MobilePhone }) != null)
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.UserPhoneAlreadyUsed,
                            $"This mobile phone number is already used by another account"));

                    user.MobilePhone = dto.MobilePhone;
                }

                //Validate password
                if(dto.Password != null)
                {
                    if (!_authManager.IsValidPassword(dto.Password))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidPassword,
                            $"Passwords must be of at least 8 characters, only English characters, " +
                            $"contains at least one digit and one special character."));

                    user.Password = _hasher.Hash(dto.Password);
                }

                //Check image
                if (dto.Image != null)
                {
                    if (!".png".Contains(Path.GetExtension(dto.Image.FileName).ToLower()))
                        return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                            ErrorCode.InvalidImageFormat,
                            $"Only .png images are allowed, The user has been created with no image."));
                    else
                    {
                        var filesPath = GetFilesPath();

                        if (!Directory.Exists(filesPath))
                            Directory.CreateDirectory(filesPath);

                        var fileName = GenerateFileName(user.Id);

                        var targetPath = Path.Combine(filesPath, fileName);
                        using (var stream = new FileStream(targetPath, FileMode.Create))
                        {
                            await dto.Image.CopyToAsync(stream);
                        }
                    }
                }

                user.FirstName = string.IsNullOrEmpty(dto.FirstName)? user.FirstName : dto.FirstName;

                user.LastName = string.IsNullOrEmpty(dto.LastName)? user.FirstName : dto.LastName;

                user.DateOfBirth = dto.DateOfBirth?? user.DateOfBirth;

                user.Address = string.IsNullOrEmpty(dto.Address) ? user.Address : dto.Address;

                var result = await _userManager.Update(user);

                return Ok(new Response<UserModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpPut("SetUserStatus")]
        public async Task<IActionResult> SetUserStatus(Guid id, UserStatus status)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return Forbid();

                var user = await _userManager.GetByIdentity(new UserRequestModel { Id = id });
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound,
                        $"No user with id = {id} was found"));
                
                user.StatusUser = status;

                var result = await _userManager.Update(user);

                return Ok(new Response<UserModel>(result, ErrorCode.NoError));
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

                var user = await _userManager.Get(id);
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound,
                        $"Unable to delete the user becuase it does not exist"));

                var filesPath = GetFilesPath();

                if (!Directory.Exists(filesPath))
                    Directory.CreateDirectory(filesPath);

                var fileName = GenerateFileName(user.Id);

                var targetPath = Path.Combine(filesPath, fileName);

                if (System.IO.File.Exists(targetPath))
                {
                    System.IO.File.Delete(targetPath);
                }

                var result = await _userManager.Delete(user);
                return Ok(new Response<UserModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }
        
        [NonAction]
        private string GetFilesPath()
        {
            var builder = new ConfigurationBuilder()
                               .SetBasePath(Directory.GetCurrentDirectory())
                               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            return builder.Build().GetSection("Path").GetSection("UsersImages").Value;
        }

        [NonAction]
        private string GenerateFileName(Guid id)
        {
            var fileName = id.ToString() + ".png";
            return fileName;
        }

        [NonAction]
        private string GetUserImage(Guid id)
        {
            var filePath = GetFilesPath();

            var imagePath = filePath + "\\" + id.ToString() + ".png";
            if (!System.IO.File.Exists(imagePath))
            {
                return string.Empty;
            }

            return imagePath;
        }

        
    }
}
