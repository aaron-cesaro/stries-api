using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;


namespace User.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        public UsersController()
        {
        }

        [HttpGet("users/health-check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult HealthCheck()
        {
            Log.Information("User Service Health-Check");

            return Ok();
        }
    }
}
