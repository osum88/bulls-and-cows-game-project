using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace bulls_and_cows_game_project.Controllers
{
    [Authorize]
    public class StartController : Controller
    {
        [HttpGet]
        public IActionResult Start(string difficulty)
        {
            // vytvoření session atd.
            return View();
        }
    }

}
