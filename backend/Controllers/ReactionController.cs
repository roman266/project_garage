using project_garage.Interfaces.IService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Models.ViewModels;
using project_garage.Data;

namespace project_garage.Controllers
{
    [Authorize]
    [Route("api/reactions")]
    [ApiController]
    public class ReactionController : ControllerBase
    {
        private readonly IReactionService _reactionService;

        public ReactionController(IReactionService reactionService)
        {
            _reactionService = reactionService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddReaction([FromBody] ReactionDto reactionDto)
        {
            try
            {
                Console.WriteLine($"Received reactionDto: {System.Text.Json.JsonSerializer.Serialize(reactionDto)}");
                if (!ModelState.IsValid)
                {
                    Console.WriteLine($"ModelState errors: {string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage))}");
                    return BadRequest(ModelState);
                }
                var currentUserId = UserHelper.GetCurrentUserId(HttpContext);
                Console.WriteLine($"Current userId: {currentUserId}");
                await _reactionService.SendReactionAsync(
                    reactionDto.EntityId,
                    reactionDto.ReactionTypeId,
                    currentUserId
                );
                Console.WriteLine("Reaction added successfully");
                return Ok(new { message = "Reaction added successfully" });
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"ArgumentException: {ex.Message}");
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unexpected error: {ex.Message}\n{ex.StackTrace}");
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        [HttpDelete("delete/{reactionId}")]
        public async Task<IActionResult> DeleteReaction(string reactionId)
        {
            try
            {
                await _reactionService.DeleteReactionAsync(reactionId);
                return Ok(new { message = "Reaction deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }

        // ReactionController.cs
        [HttpGet("entity/{entityId}")]
        public async Task<IActionResult> GetEntityReactions(string entityId)
        {
            try
            {
                var reactions = await _reactionService.GetEntityReactionsAsync(entityId);
                return Ok(new { success = true, data = reactions });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}