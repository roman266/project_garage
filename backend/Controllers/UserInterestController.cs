using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    [Route("api/interest")]
    [Authorize]
    public class UserInterestController : ControllerBase
    {
        private readonly IUserInterestService _userInterestService;

        public UserInterestController(IUserInterestService userInterestService)
        {
            _userInterestService = userInterestService;
        }

        [HttpGet("my")]
        public async Task<IActionResult> GetUserInterest()
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                var interests = await _userInterestService.GetUserInterestAsync(userId);
                return Ok(interests);
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

        [HttpPost("add")]
        public async Task<IActionResult> AddUserInterestForCurrentUser([FromBody]List<int> interestIds)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userInterestService.AddInterestAsync(userId, interestIds);
                return Ok("User interest successfully added");
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

        [HttpDelete("remove/{interestId}")]
        public async Task<IActionResult> RemoveUserInterest(int interestId)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userInterestService.RemoveInterestAsync(interestId, userId);
                return Ok("Interest successfully removed");
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
    }
}
