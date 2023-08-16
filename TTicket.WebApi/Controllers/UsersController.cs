using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using TTicket.Abstractions.Security;
using TTicket.Models.UserManagementModels;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IAuthManager _authManager;

        private readonly ILogger<UsersController> _logger;

        public UsersController(IAuthManager authManager, ILogger<UsersController> logger)
        {
            _authManager = authManager;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel model)
        {
            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            if (_authManager.IsValidPassword(model.Password))
                return BadRequest($"Phone number must start with 05");
            if (_authManager.IsValidMobileNumber(model.MobilePhone))
                return BadRequest($"Phone number must start with 05");

            var result = await _authManager.Register(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> login([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authManager.Login(model);

            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            return Ok(result);
        }
    }
}
