using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TTicket.Abstractions.Security;
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

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
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

                var result = await _authManager.Register(model);

                if (!result.IsAuthenticated)
                    return BadRequest(result.Message);

                return Ok(new Response<AuthModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }

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

                if (!(result.IsAuthenticated && result.StatusUser == 1))
                    return BadRequest(new Response<AuthModel>(result, ErrorCode.AuthenticationFailed, result.Message));

                return Ok(new Response<AuthModel>(result, ErrorCode.NoError));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An Error Occured In Controller.");
                return BadRequest(e.Message);
            }
        }
    }
}
