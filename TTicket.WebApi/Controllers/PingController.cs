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

        /// <summary>
        /// Database Health Check 
        /// </summary>
        /// <returns></returns>
        [HttpGet("DbHealthCheck")]
        public async Task<IActionResult> DbHealthCheck()
        {
            var result = await _service.CheckHealthAsync();

            if (result.Status == HealthStatus.Healthy)
                return Ok();

            return NotFound();
        }


        /// <summary>
        /// Api Health Check 
        /// </summary>
        /// <returns></returns>
        [HttpGet("ApiHealthCheck")]
        public IActionResult ApiHealthCheck()
        {
            return Ok();
        }
    }
}
