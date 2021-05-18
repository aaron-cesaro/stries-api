using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;

namespace User.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        [HttpGet("health-check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult HealthCheck()
        {
            var message = "User Service health check: ";
            return Ok(message + DateTime.UtcNow);
        }
    }
}
