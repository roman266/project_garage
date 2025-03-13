using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using System.Security.Claims;
using project_garage.Models.DbModels;
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

        [HttpGet]
        [Route("Posts/Edit/{postId}")]
        public async Task<IActionResult> EditPost(string postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return StatusCode(404, new { success = false, message = "Post not found" });
            }

            var model = new EditPostViewModel
            {
                Id = post.Id,
                Description = post.Description,
            };

            return Ok(new { success = true, post = model });
        }

        [HttpPost]
        [Route("Posts/Edit")]
        public async Task<IActionResult> EditPostSave([FromBody] EditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Invalid post data" });
            }

            try
            {
                var post = await _postService.GetPostByIdAsync(model.Id);
                if (post == null)
                {
                    return StatusCode(404, new { success = false, message = "Post not found" });
                }

                post.Description = model.Description;
                post.UpdatedAt = DateTime.UtcNow;

                await _postService.UpdatePostAsync(post);

                return Ok(new { success = true, message = "Post updated successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
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
