using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using bulls_and_cows_game_project.Services;

namespace bulls_and_cows_game_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GuessController : ControllerBase
    {
        [HttpPost]
        public IActionResult MakeGuess([FromBody] string guess)
        {
            
            var result = CodeEvaluator.EvaluateGuess("1234", guess);

            return Ok(new { bulls = result.bulls, cows = result.cows });

        }
    }
}
