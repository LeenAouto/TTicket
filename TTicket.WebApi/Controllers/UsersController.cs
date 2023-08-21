using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Mail;
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

        [Authorize(Policy = "ManagerPolicy")]
        [HttpGet("GetUsers")]
        public async Task<IActionResult> GetAll([FromQuery] UserListRequestModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(HttpContext.Session.GetString("authModel")))
                    return BadRequest("BadRequest");

                var users = await _userManager.GetList(model);
                if (!users.Any())
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" },
                        ErrorCode.UsersNotFound,
                        $"No users were found"));
                foreach (var user in users)
                {
                    user.Image = GetUserImage(user.Id);
                }

                return Ok(new Response<IEnumerable<UserModel>>(users, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }


        //// 
        //Allow the client / support user to only see his info and no other user
        //// 
        [Authorize]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            try
            {
                var uidClaim = HttpContext.User.FindFirstValue("uid");
                var userTypeClaim = HttpContext.User.FindFirstValue("TypeUser");
                if (uidClaim != id.ToString() && userTypeClaim != "1" )
                    return Forbid("Only manager account can query other users info.");
                
                var user = await _userManager.Get(id);
                if(user == null)
                    return NotFound(new Response<ErrorModel>(new ErrorModel { Message = "Not Found" }, 
                        ErrorCode.UserNotFound, 
                        $"The user was not found"));

                var userModel = new UserModel
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    MobilePhone = user.MobilePhone,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    DateOfBirth = user.DateOfBirth,
                    Address = user.Address,
                    TypeUser = user.TypeUser,
                    StatusUser = user.StatusUser,

                    TicketCount = null,
                    Image = GetUserImage(user.Id)
                };

            return Ok(new Response<User>(user, ErrorCode.NoError));
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
                var user = await _userManager.Get(id);
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request"}, 
                        ErrorCode.UserNotFound, 
                        $"Unable to update the user becuase it does not exist"));

                //Validate username
                if(!_authManager.IsValidUserName(dto.Username))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidUsername,
                        $"Usernames can only be of English characters"));

                if (dto.Username != user.Username && await _userManager.GetByIdentity(new UserRequestModel { Identity = dto.Username }) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UsernameAlreadyUsed,
                        $"This username is already used by another account"));

                user.Username = dto.Username;

                //Validate Email
                if (!_authManager.IsValidEmailAddress(dto.Email))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidEmailAddress,
                        $"Invalid email address"));

                if (dto.Email != user.Email && await _userManager.GetByIdentity(new UserRequestModel { Identity = dto.Email }) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserEmailAlreadyUsed,
                        $"This email is already used by another account"));

                user.Email = dto.Email;

                //Validate mobile number
                if (!_authManager.IsValidMobileNumber(dto.MobilePhone))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidPhoneNumber,
                        $"Phone number must start with 05 and be exactly 10 digits"));

                if (dto.MobilePhone != user.MobilePhone && await _userManager.GetByIdentity(new UserRequestModel { Identity = dto.MobilePhone }) != null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserPhoneAlreadyUsed,
                        $"This mobile phone number is already used by another account"));

                user.MobilePhone = dto.MobilePhone;

                //Validate password
                if(!_authManager.IsValidPassword(dto.Password))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidPassword,
                        $"Passwords must be of at least 8 characters, only English characters, " +
                        $"contains at least one digit and one special character."));

                if (dto.Image != null && !".png".Contains(Path.GetExtension(dto.Image.FileName).ToLower()))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidImageFormat,
                        $"Only .png images are allowed, The user has been created with no image."));

                if (dto.Image != null) 
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

                user.Password = _hasher.Hash(dto.Password);
                
                user.FirstName = string.IsNullOrEmpty(dto.FirstName)? user.FirstName : dto.FirstName;

                user.LastName = string.IsNullOrEmpty(dto.LastName)? user.FirstName : dto.LastName;

                user.DateOfBirth = dto.DateOfBirth == default? user.DateOfBirth : dto.DateOfBirth;
                user.Address = string.IsNullOrEmpty(dto.Address) ? user.Address : dto.Address;

                _userManager.Update(user);
                return Ok(new Response<User>(user, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpPut("activate/{id}")]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                var user = await _userManager.Get(id);
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound,
                        $"No user with id = {id} was found"));

                if(user.StatusUser == UserStatus.Active)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserAlreadyActive,
                        $"The user is already active"));

                user.StatusUser = UserStatus.Active;

                _userManager.Update(user);
                return Ok(new Response<User>(user, ErrorCode.NoError));

            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [Authorize(Policy = "ManagerPolicy")]
        [HttpPut("deactivate/{id}")]
        public async Task<IActionResult> Dectivate(Guid id)
        {
            try
            {
                var user = await _userManager.Get(id);
                if (user == null)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserNotFound,
                        $"Unable to deactivate the user becuase it does not exist"));

                if (user.StatusUser == UserStatus.Inactive)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.UserAlreadyInactive,
                        $"The user is already inactive"));

                user.StatusUser = UserStatus.Inactive;

                _userManager.Update(user);
                return Ok(new Response<User>(user, ErrorCode.NoError));

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

                _userManager.Delete(user);
                return Ok(new Response<User>(user, ErrorCode.NoError));
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
