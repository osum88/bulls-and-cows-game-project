using Microsoft.AspNetCore.Mvc.RazorPages;
using bulls_and_cows_game_project.Data;
using bulls_and_cows_game_project.Models;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System;

namespace bulls_and_cows_game_project.Pages
{
    public class StatisticsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public StatisticsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        public List<int> AvailableYears { get; set; } = new List<int>();

        public async Task OnGetAsync()
        {

            AvailableYears = await _context.GameSessions
                                         .Select(gs => gs.StartTime.Year)
                                         .Distinct()
                                         .OrderByDescending(year => year)
                                         .ToListAsync();

        }
    }
}