using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using TTicket.Abstractions.Security;
using TTicket.Models;
using TTicket.Models.ResponseModels;
using TTicket.Models.UserManagementModels;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthManager authManager, ILogger<AuthController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }


        [AllowAnonymous]
        [HttpPost("RegisterClient")]
        public async Task<IActionResult> RegisterClient([FromForm] RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new Response<ModelStateDictionary>(ModelState,
                        ErrorCode.InvalidModelState,
                        "ModelState is invalid"));

                if (!_authManager.IsValidUserName(model.Username))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidUsername,
                        $"Usernames can only be of English characters"));

                if (!_authManager.IsValidPassword(model.Password))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidPassword,
                        $"Passwords must be of at least 8 characters, only English characters, " +
                        $"contains at least one digit and one special character."));

                if (!_authManager.IsValidMobileNumber(model.MobilePhone))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidPhoneNumber,
                        $"Phone number must start with 05 and be exactly 10 digits"));

                if (!_authManager.IsValidEmailAddress(model.Email))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidEmailAddress,
                        $"Invalid email address"));

                if(model.Image !=  null && !".png".Contains(Path.GetExtension(model.Image.FileName).ToLower()))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidImageFormat,
                        $"Only .png images are allowed"));

                var result = await _authManager.RegisterClient(model);

                if (!result.IsAuthenticated)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                    ErrorCode.AuthenticationFailed,
                    result.Message));

                if (model.Image != null)
                {
                    var filesPath = GetFilesPath();

                    if (!Directory.Exists(filesPath))
                        Directory.CreateDirectory(filesPath);

                    var fileName = GenerateFileName(result.Id);

                    var targetPath = Path.Combine(filesPath, fileName);
                    using (var stream = new FileStream(targetPath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                }

                HttpContext.Session.SetString("authModel", JsonConvert.SerializeObject(result));

                return Ok(new Response<AuthModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }


        //[Authorize(Policy = "ManagerPolicy")]
        [HttpPost("RegisterSupport")]
        public async Task<IActionResult> RegisterSupport([FromForm] RegisterViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new Response<ModelStateDictionary>(ModelState,
                        ErrorCode.InvalidModelState,
                        "ModelState is invalid"));

                if (!_authManager.IsValidUserName(model.Username))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidUsername,
                        $"Usernames can only be of English characters"));

                if (!_authManager.IsValidPassword(model.Password))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidPassword,
                        $"Passwords must be of at least 8 characters, only English characters, " +
                        $"contains at least one digit and one special character."));

                if (!_authManager.IsValidMobileNumber(model.MobilePhone))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidPhoneNumber,
                        $"Phone number must start with 05 and be exactly 10 digits"));

                if (!_authManager.IsValidEmailAddress(model.Email))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidEmailAddress,
                        $"Invalid email address"));

                if (model.Image != null && !".png".Contains(Path.GetExtension(model.Image.FileName).ToLower()))
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.InvalidImageFormat,
                        $"Only .png images are allowed"));

                var result = await _authManager.RegisterSupport(model);

                if (!result.IsAuthenticated)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                    ErrorCode.AuthenticationFailed,
                    result.Message));

                if (model.Image != null)
                {
                    var filesPath = GetFilesPath();

                    if (!Directory.Exists(filesPath))
                        Directory.CreateDirectory(filesPath);

                    var fileName = GenerateFileName(result.Id);

                    var targetPath = Path.Combine(filesPath, fileName);
                    using (var stream = new FileStream(targetPath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }
                }

                HttpContext.Session.SetString("authModel", JsonConvert.SerializeObject(result));

                return Ok(new Response<AuthModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" },
                    ErrorCode.LoggedError, e.Message));
            }
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new Response<ModelStateDictionary>(ModelState,
                        ErrorCode.InvalidModelState,
                        "ModelState is invalid"));

                var result = await _authManager.Login(model);

                if(!result.IsAuthenticated)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AuthenticationFailed,
                        result.Message));
                else if(result.StatusUser != UserStatus.Active)
                    return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Bad Request" },
                        ErrorCode.AuthenticationFailed,
                        $"User Account is deactivated"));

                HttpContext.Session.SetString("authModel", JsonConvert.SerializeObject(result));

                return Ok(new Response<AuthModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(new Response<ErrorModel>(new ErrorModel { Message = "Logged Error" }, 
                    ErrorCode.LoggedError, e.Message));
            }
        }


        //Logout
        [Authorize]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            try
            {
                HttpContext.Session.Clear();
                
                return Ok();
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
    }
}
