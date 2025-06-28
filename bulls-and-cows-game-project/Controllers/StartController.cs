using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.CodeDom.Compiler;
using bulls_and_cows_game_project.Services;
/*
namespace bulls_and_cows_game_project.Controllers
{
    [Authorize]
    [Route("/Start")]
    public class StartController : Controller
    {
        [HttpGet]
        public IActionResult Start(string difficulty)
        {
            

            if (string.IsNullOrEmpty(difficulty) || !new[] { "easy", "normal", "hard" }.Contains(difficulty.ToLower()))
            {
                return BadRequest("Invalid difficulty.");
            }

                
                HttpContext.Session.SetString("Difficulty", difficulty);

                
                string secretCode = CodeEvaluator.GenerateSecretCode(); ;
            HttpContext.Session.SetString("SecretCode", secretCode);
                HttpContext.Session.SetInt32("RemainingAttempts", difficulty == "hard" ? 5 : (difficulty == "normal" ? 15 : int.MaxValue));

              
            
        


            return Redirect($"/Game?difficulty={difficulty}");
        }
    }

}
*/