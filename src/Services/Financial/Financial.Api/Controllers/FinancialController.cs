using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Financial.Api.Controllers
{
    [Route("v1")]
    [ApiController]
    public class FinancialController : ControllerBase
    {
        [HttpGet("health-check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult HealthCheck()
        {
            Log.Information("Financial Service Health-Check");

            return Ok("Post Service is running");
        }
    }
}
