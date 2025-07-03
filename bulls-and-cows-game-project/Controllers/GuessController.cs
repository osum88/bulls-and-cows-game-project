using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using bulls_and_cows_game_project.Services; 
using Microsoft.AspNetCore.Http; 
using bulls_and_cows_game_project.Data; 
using bulls_and_cows_game_project.Models; 
using System.Linq; 
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace bulls_and_cows_game_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class GuessController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GuessController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> MakeGuess([FromBody] string guess)
        {
            int? gameSessionId = HttpContext.Session.GetInt32("CurrentGameSessionId");
            string secretCode = HttpContext.Session.GetString("CurrentGameSecretCode");
            int? maxAttempts = HttpContext.Session.GetInt32("MaxAttempts");

            if (!gameSessionId.HasValue || string.IsNullOrEmpty(secretCode) || !maxAttempts.HasValue)
            {
                return BadRequest("Game session data not found. Please start a new game.");
            }

            // nacteni GameSession z databaze 
            var currentSession = await _context.GameSessions.FindAsync(gameSessionId.Value);
            if (currentSession == null)
            {
                return NotFound("Game session not found in database.");
            }

            var result = CodeEvaluator.EvaluateGuess(secretCode, guess);
            var isSolved = result.bulls == 4; 


            // vytvoreni noveho guess
            var newGuess = new Guess
            {
                GameSessionId = currentSession.Id,
                GuessCode = guess,
                CorrectPositionCount = result.bulls,
                WrongPositionCount = result.cows,
                CreatedAt = DateTime.UtcNow
            };
            _context.Guesses.Add(newGuess);


            currentSession.TotalGuesses++;
            currentSession.IsSolved = isSolved;
            bool resultGame = false;
            bool gameOver = (maxAttempts != int.MaxValue && currentSession.TotalGuesses >= maxAttempts);
            string formattedResultTime = "00:00:00";

            if (isSolved || gameOver)
            {
                currentSession.EndTime = DateTime.UtcNow;
                TimeSpan resultTime = currentSession.EndTime - currentSession.StartTime;
                formattedResultTime = resultTime.ToString(@"hh\:mm\:ss");

                if (isSolved)
                {
                    resultGame = true;
                }
                
            }

            await _context.SaveChangesAsync();

            return Ok(new
            {
                bulls = result.bulls,
                cows = result.cows,
                isEndGame = isSolved || (currentSession.TotalGuesses >= maxAttempts),
                resultGame = resultGame,
                attempts = currentSession.TotalGuesses,
                resultTime = formattedResultTime,
                secretCode = isSolved || (gameOver && !isSolved) ? secretCode : null
            });
        }

        [HttpPost("EndGame")] 
        public async Task<IActionResult> EndGame()
        {
            int? gameSessionId = HttpContext.Session.GetInt32("CurrentGameSessionId");

            if (!gameSessionId.HasValue)
            {
                return BadRequest("No active game session to end.");
            }

            var currentSession = await _context.GameSessions.FindAsync(gameSessionId.Value);

            if (currentSession == null)
            {
                return NotFound("Game session not found in database.");
            }

            currentSession.IsSolved = false; 

            string secretCode = HttpContext.Session.GetString("CurrentGameSecretCode");

            currentSession.EndTime = DateTime.UtcNow;
            TimeSpan resultTime = currentSession.EndTime - currentSession.StartTime;
            string formattedResultTime = resultTime.ToString(@"hh\:mm\:ss");

            await _context.SaveChangesAsync();

            HttpContext.Session.Remove("CurrentGameSessionId");
            HttpContext.Session.Remove("CurrentGameSecretCode");
            HttpContext.Session.Remove("MaxAttempts");

            return Ok(new
            {
                bulls = 0, 
                cows = 0, 
                isEndGame = true, 
                resultGame = false, 
                attempts = currentSession.TotalGuesses,
                resultTime = formattedResultTime,
                secretCode = secretCode 
            });
        }
    }
}