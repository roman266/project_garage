using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using System.Security.Claims;
using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Authorization;

namespace project_garage.Controllers
{
    [Authorize]
    public class PostsController : Controller
    {
        private readonly IPostService _postService;

        public PostsController(IPostService postService)
        {
            _postService = postService;
        }

        [HttpPost]
        [Route("Posts/Create")]
        public async Task<IActionResult> CreatePost(CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid post data");
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return Unauthorized();
            }

            var post = new PostModel
            {
                Title = model.Title,
                Description = model.Description,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _postService.CreatePostAsync(post);

            return RedirectToAction("ProfileIndex", "Profile", new { userId = userId });
        }

        [HttpGet]
        [Route("Posts/Edit/{postId}")]
        public async Task<IActionResult> EditPost(Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return NotFound();
            }

            var model = new EditPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description
            };

            return View("EditPost", model); //без явного вказання не працює
        }

        [HttpPost]
        [Route("Posts/Edit")]
        public async Task<IActionResult> EditPostSave(EditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var post = await _postService.GetPostByIdAsync(model.Id);
                post.Title = model.Title;
                post.Description = model.Description;
                post.UpdatedAt = DateTime.UtcNow;

                await _postService.UpdatePostAsync(post);
                return RedirectToAction("ProfileIndex", "Profile", new { userId = post.UserId });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error occurred while updating the post.");
            }
        }

        [HttpPost]
        [Route("Posts/Delete")]
        public async Task<IActionResult> DeletePost(Guid postId)
        {
            try
            {
                var post = await _postService.GetPostByIdAsync(postId);
                if (post == null)
                {
                    return NotFound();
                }

                await _postService.DeletePostAsync(postId);
                return RedirectToAction("ProfileIndex", "Profile", new { userId = post.UserId });
            }
            catch (Exception)
            {
                return StatusCode(500, "Error occurred while deleting the post.");
            }
        }
    }
}
