using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using project_garage.Interfaces.IRepository;
using Microsoft.AspNetCore.Authorization;
using project_garage.Data;

namespace project_garage.Controllers
{
    [Route("api/post")]
    [Authorize]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost("create")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> CreatePost([FromForm] PostOnCreationDto model)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _postService.CreatePostAndUploadImageToCloudAsync(userId, model);
                return Ok(new { message = "Post created successfully"});
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpGet("my-posts")]
        public async Task<IActionResult> GetMyPosts(string? lastPostId, int limit = 15)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var posts = await _postService.GetPaginatedPostsByUserIdAsync(userId, lastPostId, limit);
                return Ok(posts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
        [HttpGet("user-posts")]
        public async Task<IActionResult> GetUserPosts(string userId, string? lastPostId, int limit = 15)
        {
            try
            {
                var posts = await _postService.GetPaginatedPostsByUserIdAsync(userId, lastPostId, limit);
                return Ok(posts);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPatch("edit/{postId}")]
        public async Task<IActionResult> EditPost([FromBody]EditPostDto editPostDto)
        {
            try
            {
                await _postService.UpdatePostAsync(editPostDto);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpDelete("delete/{postId}")]
        public async Task<IActionResult> DeletePost(string postId)
        { 
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return StatusCode(404, new { success = false, message = "No post with this id" });
            }

            await _postService.DeletePostAsync(postId);
            return Ok(new { success = true, message = "Post deleted successfully" });
        }
    }
}
