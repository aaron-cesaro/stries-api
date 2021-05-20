using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Models;
using Post.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostManager _postManager;

        public PostsController(IPostManager postManager)
        {
            _postManager = postManager;
        }

        [HttpGet("posts/health-check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult HealthCheck()
        {
            Log.Information("Post Service Health-Check");

            return Ok();
        }

        [HttpPost("posts")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> CreatePostAsync([FromBody] CreatePostRequest postRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var postId = Guid.Empty;

            try
            {
                postId = await _postManager.CreatePostAsync(postRequest);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postRequest.Title} cannot be created");

                return StatusCode(500);
            }

            return Ok(postId);
        }

        [HttpPut("posts/{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> SaveAndPublishPostAsync(Guid postId, string status, [FromBody] PostData newPostData)
        {
            if (!ModelState.IsValid || postId == Guid.Empty || (status != "draft" && status != "published"))
                return BadRequest();

            try
            {
                await _postManager.SaveAndPusblishPostAsync(postId, status, newPostData);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be updated");

                return StatusCode(500);
            }

            return Ok();
        }

        [HttpDelete("posts/{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> DeletePostAsync(Guid postId)
        {
            if (postId == Guid.Empty)
                return BadRequest();

            try
            {
                await _postManager.RemovePostAsync(postId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {postId} cannot be deleted");

                return StatusCode(500);
            }

            return Ok();
        }
    }
}
