using Microsoft.AspNetCore.Mvc;
using project_garage.Interfaces.IRepository;

namespace project_garage.Controllers
{
    [Route("api/interest")]
    public class InterestController : ControllerBase
    {
        private readonly IInterestRepository _interestRepository;

        public InterestController(IInterestRepository interestRepository) 
        {
            _interestRepository = interestRepository;
        }

        [HttpGet("get-all")]
        public IActionResult GetAllInterests()
        {
            var interests = _interestRepository.GetAllInterests();
            if (!interests.Any())
            {
                return StatusCode(500, "No interests founded");
            }
            return Ok(interests);
        }
    }
}
