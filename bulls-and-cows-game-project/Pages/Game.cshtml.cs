using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using bulls_and_cows_game_project.Data;
using bulls_and_cows_game_project.Models;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic; 

namespace bulls_and_cows_game_project.Pages
{
    [Authorize]
    public class GameModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Player> _userManager;

        public GameModel(ApplicationDbContext context, UserManager<Player> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [BindProperty(SupportsGet = true)]
        public string Difficulty { get; set; }
        public int MaxAttempts { get; set; }

        public int CurrentGameSessionId { get; set; }
        public string CurrentGameSecretCode { get; set; }


        public async Task OnGetAsync()
        {
            var currentPlayer = await _userManager.GetUserAsync(User);
            if (currentPlayer == null)
            {
                RedirectToPage("/Account/Login");
                return;
            }

            await CleanUpIncompleteGameSessions();

            // --- 1. Nastaven� MaxAttempts na z�klad� obt�nosti a ur�en� n�zvu tagu ---
            string difficultyTagName = ""; 
            switch (Difficulty?.ToLower())
            {
                case "normal":
                    MaxAttempts = 15;
                    difficultyTagName = "Normal";
                    break;
                case "hard":
                    MaxAttempts = 5;
                    difficultyTagName = "Hard";
                    break;
                case "easy":
                default:
                    MaxAttempts = int.MaxValue;
                    difficultyTagName = "Easy";
                    break;
            }

            HttpContext.Session.SetInt32("MaxAttempts", MaxAttempts);

            // --- 2. Z�sk�n� nebo vytvo�en� tagu obt�nosti ---
            Tag difficultyTag = await _context.Tags.FirstOrDefaultAsync(t => t.Name == difficultyTagName);

            if (difficultyTag == null)
            {
                difficultyTag = new Tag { Name = difficultyTagName };
                _context.Tags.Add(difficultyTag);
                await _context.SaveChangesAsync(); 
            }


            // --- 3. Vygenerujte c�lov� k�d pro novou hru ---
            string secretCode = GenerateSecretCode();


            // --- 4. Vytvo�te a ulo�te novou GameSession ---
            var newGameSession = new GameSession
            {
                PlayerId = currentPlayer.Id,
                StartTime = DateTime.UtcNow,
                TargetCode = secretCode,
                IsSolved = false,
                TotalGuesses = 0,
                GameSessionTags = new List<GameSessionTag>() // Inicializujte kolekci
            };

            // P�idejte GameSessionTag pro obt�nost
            newGameSession.GameSessionTags.Add(new GameSessionTag
            {
                Tag = difficultyTag 
            });

            _context.GameSessions.Add(newGameSession);
            await _context.SaveChangesAsync(); // Ulo�te relaci a GameSessionTag do DB


            // --- 5. Ulo�te d�le�it� informace do Session pro GuessController ---
            HttpContext.Session.SetInt32("CurrentGameSessionId", newGameSession.Id);
            HttpContext.Session.SetString("CurrentGameSecretCode", secretCode);
            HttpContext.Session.SetInt32("CurrentGameGuessesMade", 0);


            // --- 6. P�edejte data do Razor Page View ---
            CurrentGameSessionId = newGameSession.Id;
            CurrentGameSecretCode = secretCode;
        }

        private async Task CleanUpIncompleteGameSessions()
        {
            var incompleteSessions = await _context.GameSessions
                .Where(gs => !gs.IsSolved && gs.EndTime == default(DateTime))
                .Include(gs => gs.Guesses)
                .ToListAsync();

            if (incompleteSessions.Any())
            {
                _context.GameSessions.RemoveRange(incompleteSessions);
                await _context.SaveChangesAsync();
            }
        }

        public static string GenerateSecretCode()
        {
            var rnd = new Random();
            var digits = "0123456789".ToCharArray();

            for (int i = 9; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (digits[i], digits[j]) = (digits[j], digits[i]);
            }

            return new string(digits, 0, 4);
        }
    }
}