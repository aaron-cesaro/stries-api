using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Post.Api.Application.Models;
using Post.Api.Infrastructure.Exceptions;
using Post.Api.Interfaces;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Post.Api.Controllers
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

            return Ok("Post Service is running");
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

            try
            {
                var postId = await _postManager.CreatePostAsync(postRequest);

                if (postId == Guid.Empty)
                    return StatusCode(500);

                return Ok(postId);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post {postRequest.Title} cannot be created");

                return ex switch
                {
                    AuthorNotFoundException => NotFound($"Post {postRequest.Title}, author not found"),

                    _ => StatusCode(500)
                };
            }
        }

        [HttpGet("{id:guid}")]
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
                    PostNotFoundException => NotFound($"Post {id} not found"),
                    AuthorNotFoundException => NotFound($"Post {id}, author not found"),

                    _ => StatusCode(500)
                };
            }
        }

        [HttpPut("{id:guid}")]
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
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be saved");

                return ex switch
                {
                    PostNotFoundException => NotFound($"Post {id} not found"),
                    AuthorNotFoundException => NotFound($"Post {id}, author not found"),
                    PostAlreadyPublishedException => BadRequest($"Post {id} already published"),

                    _ => StatusCode(500)
                };
            }

            return Ok($"Post {id} successfully saved");
        }

        [HttpPut("{id:guid}/{status:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        [ProducesResponseType(500)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> UpdatePostStatusAsync(Guid id, int status)
        {
            if (!ModelState.IsValid || id == Guid.Empty || (status != (int)PostStatus.published && status != (int)PostStatus.archived))
                return BadRequest();

            var newStatus = string.Empty;

            try
            {
                if (status == (int)PostStatus.published)
                {
                    await _postManager.PublishPostAsync(id);
                    newStatus = "published";
                }
                else if (status == (int)PostStatus.archived)
                {
                    await _postManager.ArchivePostAsync(id);
                    newStatus = "archived";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be published or archived");

                return ex switch
                {
                    PostNotFoundException => NotFound($"Post {id} cannot be {newStatus} because post is not found"),
                    PostAlreadyArchivedException => BadRequest($"Post {id} cannot be {newStatus} because post is already archived"),
                    AuthorNotFoundException => NotFound($"Post {id} cannot be {newStatus} because post is, author not found"),

                    _ => StatusCode(500)
                };
            }

            return Ok($"Post {id} successfully {newStatus}");
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
                await _postManager.DeletePostByIdAsync(id);
            }
            catch (Exception ex)
            {
                Log.Error(ex, ex.Message, $"Post with id {id} cannot be deleted");

                return ex switch
                {
                    PostNotFoundException => NotFound($"Post {id} not found"),

                    _ => StatusCode(500)
                };
            }

            return Ok($"Post {id} successfully deleted");
        }
    }
}
