using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using bulls_and_cows_game_project.Data;
using bulls_and_cows_game_project.Models;
using System.Security.Claims;
using System;
using Microsoft.AspNetCore.Authorization;
using System.Xml.Serialization;

namespace bulls_and_cows_game_project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] 
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetGameStatistics(
            [FromQuery] string? year,    
            [FromQuery] string? month,    
            [FromQuery] string? day,   
            [FromQuery] string? status,
            [FromQuery] string? difficulty,
            [FromQuery] string sortBy = "startTime", 
            [FromQuery] string sortDir = "desc",
            [FromQuery] bool download = false)     
        {
            // ziska ID uživatele z jeho claims
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            IQueryable<GameSession> query = _context.GameSessions
                                                .Where(gs => gs.EndTime != default(DateTime))
                                                .Include(gs => gs.GameSessionTags)
                                                    .ThenInclude(gst => gst.Tag)
                                                .Include(gs => gs.Guesses);

            // filtr podle uzivatele
            if (userId != null)
            {
                query = query.Where(gs => gs.PlayerId == userId);
            }

            // filtr podle roku
            if (!string.IsNullOrEmpty(year) && year.ToLower() != "all")
            {
                if (int.TryParse(year, out int y))
                {
                    query = query.Where(gs => gs.StartTime.Year == y);
                }
            }

            // filtr podle měsíce
            if (!string.IsNullOrEmpty(month) && month.ToLower() != "all")
            {
                if (int.TryParse(month, out int m))
                {
                    query = query.Where(gs => gs.StartTime.Month == m);
                }
            }

            // filtr podle dne
            if (!string.IsNullOrEmpty(day) && day.ToLower() != "all")
            {
                if (int.TryParse(day, out int d))
                {
                    query = query.Where(gs => gs.StartTime.Day == d);
                }
            }

            // filtr podle statusu
            if (!string.IsNullOrEmpty(status) && status.ToLower() != "all")
            {
                if (status.ToLower() == "won")
                {
                    query = query.Where(gs => gs.IsSolved);
                }
                else if (status.ToLower() == "lost")
                {
                    query = query.Where(gs => !gs.IsSolved);
                }
            }

            // filtr podle obtížnosti
            if (!string.IsNullOrEmpty(difficulty) && difficulty.ToLower() != "all" && difficulty.ToLower() != "null")
            {
                query = query.Where(gs => gs.GameSessionTags.Any(gst => gst.Tag.Name == difficulty));
            }

            // razeni
            switch (sortBy?.ToLower())
            {
                case "starttime":
                    query = sortDir?.ToLower() == "asc" ? query.OrderBy(gs => gs.StartTime) : query.OrderByDescending(gs => gs.StartTime);
                    break;
                case "duration":
                    query = (sortDir?.ToLower() == "asc") ? query.OrderBy(gs => EF.Functions.DateDiffSecond(gs.StartTime, gs.EndTime)) : 
                                                            query.OrderByDescending(gs => EF.Functions.DateDiffSecond(gs.StartTime, gs.EndTime)); 
                    break;
                case "issolved":
                    query = sortDir?.ToLower() == "asc" ? query.OrderBy(gs => gs.IsSolved) : query.OrderByDescending(gs => gs.IsSolved);
                    break;
                case "targetcode":
                    query = sortDir?.ToLower() == "asc" ? query.OrderBy(gs => gs.TargetCode) : query.OrderByDescending(gs => gs.TargetCode);
                    break;
                case "totalguesses":
                    query = sortDir?.ToLower() == "asc" ? query.OrderBy(gs => gs.TotalGuesses) : query.OrderByDescending(gs => gs.TotalGuesses);
                    break;
                case "difficulty":
                    query = sortDir?.ToLower() == "asc" ?
                            query.OrderBy(gs => gs.GameSessionTags.Select(gst => gst.Tag.Name).FirstOrDefault()) :
                            query.OrderByDescending(gs => gs.GameSessionTags.Select(gst => gst.Tag.Name).FirstOrDefault());
                    break;
                default:
                    query = query.OrderByDescending(gs => gs.StartTime);
                    break;
            }
            // dotaz na databazi a výsledky do seznamu objektů GameSession
            var games = await query.ToListAsync();

            // entity GameSession do DTO 
            var gameStatistics = games.Select(gs => new GameStatistics
            {
                Id = gs.Id,
                StartTime = gs.StartTime,
                EndTime = gs.EndTime,
                Duration = (gs.EndTime - gs.StartTime).ToString(@"hh\:mm\:ss"),
                TargetCode = gs.TargetCode,
                TotalGuesses = gs.TotalGuesses,
                IsSolved = gs.IsSolved,
                Difficulties = gs.GameSessionTags.Select(gst => gst.Tag.Name).ToList(),
                Guesses = gs.Guesses.Select(g => new GuessStatistics 
                {
                    GuessCode = g.GuessCode,
                    CorrectPositionCount = g.CorrectPositionCount,
                    WrongPositionCount = g.WrongPositionCount,
                    CreatedAt = g.CreatedAt
                }).OrderBy(g => g.CreatedAt).ToList() 
            }).ToList();


            if (download)
            {
                // vytvori MemoryStream pro zápis XML.
                MemoryStream memoryStream = new MemoryStream();  
                try
                {
                    using (TextWriter tw = new StreamWriter(memoryStream, System.Text.Encoding.UTF8, 1024, true))
                    {
                        // inicializuje XmlSerializer pro seznam objektů GameStatistics
                        XmlSerializer serializer = new XmlSerializer(typeof(List<GameStatistics>));
                        // serializuje seznam gameStatistics do TextWriteru
                        serializer.Serialize(tw, gameStatistics);
                    }

                    memoryStream.Position = 0;

                    // vrati XML soubor jako HTTP odpověď
                    return File(memoryStream, "application/xml", "statistics.xml");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error during XML generation: {ex.Message}");
                    return StatusCode(500, "Error generating XML statistics file.");
                }
            }

            return Ok(gameStatistics);
        }
    }

    public class GameStatistics
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Duration { get; set; } 
        public string TargetCode { get; set; } = string.Empty;
        public int TotalGuesses { get; set; }
        public bool IsSolved { get; set; }
        public List<string> Difficulties { get; set; } = new List<string>();
        public List<GuessStatistics> Guesses { get; set; } = new List<GuessStatistics>();

        
    }

    public class GuessStatistics
    {
        public string GuessCode { get; set; }
        public int CorrectPositionCount { get; set; }
        public int WrongPositionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }

}