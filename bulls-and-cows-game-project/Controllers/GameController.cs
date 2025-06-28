using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CodeDom.Compiler;
using bulls_and_cows_game_project.Services;/*

namespace bulls_and_cows_game_project.Controllers
{
    [Authorize]
    [Route("/Game")]
    public class GameController : Controller
    {
        [HttpGet]
        public IActionResult Game(string difficulty)
        {
            

            if (string.IsNullOrEmpty(difficulty) || !new[] { "easy", "normal", "hard" }.Contains(difficulty.ToLower()))
            {
                return BadRequest("Invalid difficulty.");
            }


            HttpContext.Session.SetString("Difficulty", difficulty);


            string secretCode = CodeEvaluator.GenerateSecretCode(); ;
            HttpContext.Session.SetString("SecretCode", secretCode);
            HttpContext.Session.SetInt32("RemainingAttempts", difficulty == "hard" ? 5 : (difficulty == "normal" ? 15 : int.MaxValue));
            


            return View("Game");



            //  return Redirect($"/Game?difficulty={difficulty}");
        }
    }

}*/
