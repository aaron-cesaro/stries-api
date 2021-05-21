using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Application.Models;
using Post.Infrastructure.Exceptions;
using Post.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Controllers
{
    [Route("v1")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostManager _postManager;

        public PostsController(IPostManager postManager)
        {
            _postManager = postManager; 
        }

        [HttpGet("health-check")]
        [ProducesResponseType(200)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public IActionResult HealthCheck()
        {
            Log.Information("Post Service Health-Check");

            return Ok();
        }

        [HttpPost]
        [ProducesResponseType(typeof(Guid), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> CreatePostAsync([FromBody] PostCreateRequest postRequest)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var postId = Guid.Empty;

            try
            {
                postId = await _postManager.CreatePostAsync(postRequest);

                return Ok(postId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postRequest.Title} cannot be created");

                return StatusCode(500);
            }
        }

        [HttpGet("{Id:guid}")]
        [ProducesResponseType(typeof(PostReponse), 200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> GetPostAsync(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            try
            {
                var post = await _postManager.GetPostByIdAsync(id);

                return Ok(post);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be retrieved");

                return ex switch
                {
                    PostNotFoundException e => NotFound(),

                    _ => StatusCode(500)
                };
            }
        }

        [HttpPost("{Id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> SavePostAsync(Guid id, [FromBody] PostData newPostData)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
                return BadRequest();

            try
            {
                await _postManager.SavePostAsync(id, newPostData);
            }
            catch(Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be saved");

                return ex switch
                {
                    PostNotFoundException e => NotFound(),
                    PostAlreadyPublishedException e => BadRequest(),

                    _ => StatusCode(500)
                };
            }

            return Ok();
        }

        [HttpPut("{Id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> PublishPostAsync(Guid id)
        {
            if (!ModelState.IsValid || id == Guid.Empty)
                return BadRequest();

            try
            {
                await _postManager.PublishPostAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be published");

                return ex switch
                {
                    PostNotFoundException e => NotFound(),
                    PostAlreadyPublishedException e => BadRequest(),

                    _ => StatusCode(500)
                };
            }

            return Ok();
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> DeletePostAsync(Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            try
            {
                await _postManager.RemovePostAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be deleted");

                return ex switch
                {
                    PostNotFoundException e => NotFound(),

                    _ => StatusCode(500)
                };
            }

            return Ok();
        }
    }
}
