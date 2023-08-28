using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using System.Security.Claims;
using TTicket.Abstractions.DAL;
using TTicket.Abstractions.Security;
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

        /// <summary>
        /// Get all users information (Used by manager only)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Policy = "ManagerPolicy")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetAll([FromQuery] UserListRequestModel model)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

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

        /// <summary>
        /// Get the details about a specific user (a non-manager user can only retrive his own information)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");
                if (currentUserId != id && CurrentUserType != "1")
                    return Forbid();

                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();
                //else
                //{
                //    var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                //    var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");
                //    if (currentUserId != id && CurrentUserType != "1")
                //        return Forbid();
                //}

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

        /// <summary>
        /// Get the user image (a non-manager user can only retrive his own image)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet("GetImage/{id}")]
        public async Task<IActionResult> GetImage(Guid id)
        {
            try
            {
                var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");
                if (CurrentUserType != "1")
                {
                    if(currentUserId != id)
                        return Forbid();
                }
                    

                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();
                //else
                //{
                //    var currentUserId = Guid.Parse(HttpContext.User.FindFirstValue("uid"));
                //    var CurrentUserType = HttpContext.User.FindFirstValue("TypeUser");
                //    if (currentUserId != id && CurrentUserType != "1")
                //        return Forbid();
                //}

                var user = await _userManager.Get(id);
                if (user == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UserNotFound,
                        $"No user was found"));

                var targetPath = GetUserImage(user.Id);

                if(string.IsNullOrEmpty(targetPath))
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UserImageNotFound,
                        $"No image of this user was found"));

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
        /// Update user information (Only used by manager)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="dto"></param>
        /// <returns></returns>
        [Authorize(Policy = "ManagerPolicy")]
        [HttpPut("{id}")] //used by manager
        public async Task<IActionResult> UpdateUser(Guid id, [FromForm] UserUpdateDto dto)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

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
                    var filesPath = GetFilesPath();

                    if (!Directory.Exists(filesPath))
                        Directory.CreateDirectory(filesPath);

                    var fileName = GenerateFileName(user.Id, dto.Image);

                    var targetPath = Path.Combine(filesPath, fileName);
                    using (var stream = new FileStream(targetPath, FileMode.Create))
                    {
                        await dto.Image.CopyToAsync(stream);
                    }
                }

                user.FirstName = string.IsNullOrEmpty(dto.FirstName)? user.FirstName : dto.FirstName;

                user.LastName = string.IsNullOrEmpty(dto.LastName)? user.LastName : dto.LastName;

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

        /// <summary>
        /// Activate and deactivate the user (Only used by manager)
        /// </summary>
        /// <param name="id"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [Authorize(Policy = "ManagerPolicy")]
        [HttpPut("SetUserStatus")]
        public async Task<IActionResult> SetUserStatus(Guid id, UserStatus status)
        {
            try
            {
                //if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                //    return Forbid();

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

        /// <summary>
        /// Delete a user account from the system (Only used by manager)
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

                var user = await _userManager.Get(id);
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound,
                        $"Unable to delete the user becuase it does not exist"));

                //var filesPath = GetFilesPath();

                //if (!Directory.Exists(filesPath))
                //    Directory.CreateDirectory(filesPath);

                //var fileName = GenerateFileName(user.Id);

                //var targetPath = Path.Combine(filesPath, fileName);

                //if (System.IO.File.Exists(targetPath))
                //{
                //    System.IO.File.Delete(targetPath);
                //}

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
        private string GenerateFileName(Guid id, IFormFile file)
        {
            var extension = "." + file.FileName.Split('.').Last();

            var fileName = id.ToString() + extension;

            return fileName;
        }

        [NonAction]
        private string GetUserImage(Guid id)
        {
            string filesPath = GetFilesPath();

            string[] matchingFiles = Directory.GetFiles(filesPath, id.ToString()+"*");

            if (matchingFiles.Length == 0)
            {
                return string.Empty;
            }

            // Select the first matching file (you might want to handle multiple matches differently)
            string filePath = matchingFiles[0];

            return filePath;
        }

        
    }
}
