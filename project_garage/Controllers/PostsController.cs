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

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpPost]
        [Route("Posts/Create")]
        public async Task<IActionResult> CreatePost([FromBody] CreatePostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponse(new { success = false, message = "Invalid post data" }, 400);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userId == null)
            {
                return JsonResponse(new { success = false, message = "Unauthorized" }, 401);
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

            if (model.ImageUrls != null && model.ImageUrls.Any())
            {
                await _postService.AddImagesToPostAsync(post.Id, model.ImageUrls);
            }

            return JsonResponse(new { success = true, message = "Post created successfully", postId = post.Id });
        }

        [HttpGet]
        [Route("Posts/Edit/{postId}")]
        public async Task<IActionResult> EditPost(Guid postId)
        {
            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return JsonResponse(new { success = false, message = "Post not found" }, 404);
            }

            var model = new EditPostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Description = post.Description,
                //ImageUrls = post.Images.Select(i => i.ImageUrl).ToList()
            };

            return JsonResponse(new { success = true, post = model });
        }

        [HttpPost]
        [Route("Posts/Edit")]
        public async Task<IActionResult> EditPostSave([FromBody] EditPostViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return JsonResponse(new { success = false, message = "Invalid post data" }, 400);
            }

            try
            {
                var post = await _postService.GetPostByIdAsync(model.Id);
                if (post == null)
                {
                    return JsonResponse(new { success = false, message = "Post not found" }, 404);
                }

                post.Title = model.Title;
                post.Description = model.Description;
                post.UpdatedAt = DateTime.UtcNow;

                await _postService.UpdatePostAsync(post);

                return JsonResponse(new { success = true, message = "Post updated successfully" });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("Posts/Delete")]
        public async Task<IActionResult> DeletePost([FromBody] Dictionary<string, string> request)
        {
            if (request == null || !request.ContainsKey("postId"))
            {
                return JsonResponse(new { success = false, message = "Invalid post ID" }, 400);
            }

            if (!Guid.TryParse(request["postId"], out Guid postId) || postId == Guid.Empty)
            {
                return JsonResponse(new { success = false, message = "Invalid post ID" }, 400);
            }

            var post = await _postService.GetPostByIdAsync(postId);
            if (post == null)
            {
                return JsonResponse(new { success = false, message = "No post with this id" }, 404);
            }

            await _postService.DeletePostAsync(postId);
            return JsonResponse(new { success = true, message = "Post deleted successfully" });
        }

    }
}
