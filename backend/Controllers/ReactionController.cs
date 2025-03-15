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
                var currentUserId = UserHelper.GetCurrentUserId(HttpContext);

                await _reactionService.SendReactionAsync(
                    reactionDto.EntityId,
                    reactionDto.ReactionType,
                    reactionDto.EntityType,
                    currentUserId
                );

                return Ok(new { message = "Reaction added successfully" });
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

        [HttpDelete("delete/{reactionId}")]
        public async Task<IActionResult> DeleteReaction(string reactionId) 
        {
            try
            {
                await _reactionService.DeleteReactionAsync(reactionId);
                return Ok(new { message = "Message deleted successfully" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new {error  = ex.Message});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An unexpected error occurred.", details = ex.Message });
            }
        }
    }
}
