using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using bulls_and_cows_game_project.Services; 
using Microsoft.AspNetCore.Http; 
using bulls_and_cows_game_project.Data; 
using bulls_and_cows_game_project.Models; 
using System.Linq; 
using System.Threading.Tasks; 

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

            // validace tipu
            if (string.IsNullOrEmpty(guess) || guess.Length != 4 || !guess.All(char.IsDigit) || guess.Distinct().Count() != 4)
            {
                return BadRequest("Invalid guess format. Please enter 4 unique digits.");
            }

            // kontrola jestli hra neni jiz vyresena nebo je prekrocen limit pokusu
            if (currentSession.IsSolved)
            {
                return Ok(new { error = "Game is already solved.", isSolved = true, secretCode = secretCode });
            }
            if (maxAttempts != int.MaxValue && currentSession.TotalGuesses >= maxAttempts)
            {
                return Ok(new { error = "Maximum attempts reached.", isSolved = false, resultGame = false, secretCode = secretCode });
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
    }
}