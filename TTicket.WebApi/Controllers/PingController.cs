using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace TTicket.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PingController : ControllerBase
    {
        private readonly HealthCheckService _service;

        public PingController(HealthCheckService service)
        {
            _service = service;
        }

        [HttpGet("DbHealthCheck")]
        public async Task<IActionResult> DbHealthCheck()
        {
            var result = await _service.CheckHealthAsync();

            if (result.Status == HealthStatus.Healthy)
                return Ok();

            return NotFound();
        }

        [HttpGet("ApiHealthCheck")]
        public IActionResult ApiHealthCheck()
        {
            return Ok();
        }
    }
}
