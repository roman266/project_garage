using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Interfaces.IService;
using System.Security.Claims;
using project_garage.Models.DbModels;
using Microsoft.AspNetCore.Authorization;

namespace project_garage.Controllers
{
    [Authorize]
    public class CommentsController : Controller
    {
        private readonly ICommentService _commentService;

        public CommentsController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        private IActionResult JsonResponse(object data, int statusCode = 200)
        {
            Response.StatusCode = statusCode;
            return Json(data);
        }

        [HttpPost]
        [Route("Comments/Create")]
        public async Task<IActionResult> CreateComment([FromBody] CreateCommentViewModel model)
        {
            Console.WriteLine($"PostId: {model.PostId}, Content: {model.Content}");

            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Content))
            {
                Console.WriteLine("ModelState is invalid!");
                return BadRequest(new { success = false, message = "Invalid comment data.", errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                Console.WriteLine("User is unauthorized!");
                return Unauthorized(new { success = false, message = "Unauthorized" });
            }

            try
            {
                var comment = await _commentService.AddCommentAsync(model.PostId, userId, model.Content);
                Console.WriteLine("Comment created successfully!");
                return Ok(new { success = true, message = "Comment created successfully." });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }


        private (bool flowControl, IActionResult value) NewMethod(CreateCommentViewModel model)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(model.Content))
            {
                return (flowControl: false, value: BadRequest(new { success = false, message = "Invalid comment data." }));
            }

            return (flowControl: true, value: null);
        }

        [HttpGet]
        [Route("Comments/Post/{postId}")]
        public async Task<IActionResult> GetCommentsByPostId(string postId)
        {
            try
            {
                var comments = await _commentService.GetCommentsByPostIdAsync(postId);
                return JsonResponse(new { success = true, comments });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }

        [HttpPost]
        [Route("Comments/Delete")]
        public async Task<IActionResult> DeleteComment([FromBody] DeleteCommentViewModel model)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(model.CommentId);
                if (comment == null)
                {
                    return JsonResponse(new { success = false, message = "Comment not found." }, 404);
                }

                await _commentService.DeleteCommentAsync(model.CommentId);
                return JsonResponse(new { success = true, message = "Comment deleted successfully." });
            }
            catch (Exception ex)
            {
                return JsonResponse(new { success = false, message = ex.Message }, 500);
            }
        }


        [HttpGet]
        [Route("Comments/Edit/{commentId}")]
        public async Task<IActionResult> EditComment(int commentId)
        {
            try
            {
                var comment = await _commentService.GetCommentByIdAsync(commentId);
                if (comment == null)
                {
                    return NotFound(new { success = false, message = "Comment not found." });
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId) || comment.UserId != userId)
                {
                    return Unauthorized(new { success = false, message = "Unauthorized" });
                }

                var model = new EditCommentViewModel
                {
                    CommentId = comment.Id,
                    Content = comment.Content,
                    PostId = comment.PostId
                };

                return Ok(new { success = true, model });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        [Route("Comments/Edit")]
        public async Task<IActionResult> EditComment([FromBody] EditCommentViewModel model)
        {
            // Логування вхідних даних для діагностики
            Console.WriteLine($"CommentId: {model.CommentId}");
            Console.WriteLine($"Content: {model.Content}");
            Console.WriteLine($"PostId: {model.PostId}");

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                return BadRequest(new { success = false, message = "Invalid comment data.", errors });
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized(new { success = false, message = "Unauthorized: User not authenticated." });
            }

            try
            {
                await _commentService.UpdateCommentAsync(model.CommentId, userId, model.Content);
                return Ok(new { success = true, message = "Comment updated successfully." });
            }
            catch (UnauthorizedAccessException)
            {
                return Unauthorized(new { success = false, message = "Unauthorized: You cannot edit this comment." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Internal server error", details = ex.Message });
            }
        }
    }
}
