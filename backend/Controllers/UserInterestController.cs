﻿using Microsoft.AspNetCore.Mvc;
using project_garage.Data;
using project_garage.Interfaces.IService;

namespace project_garage.Controllers
{
    [Route("api/interest")]
    public class UserInterestController : ControllerBase
    {
        private readonly IUserInterestService _userInterestService;

        public UserInterestController(IUserInterestService userInterestService)
        {
            _userInterestService = userInterestService;
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserInterest(string userId)
        {
            try
            {
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
        public async Task<IActionResult> AddUserInterestForCurrentUser(List<string> interests)
        {
            try
            {
                var userId = UserHelper.GetCurrentUserId(HttpContext);
                await _userInterestService.AddInterestAsync(userId, interests);
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
        public async Task<IActionResult> RemoveUserInterest(string interestId)
        {
            try
            {
                await _userInterestService.RemoveInterestAsync(interestId);
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
