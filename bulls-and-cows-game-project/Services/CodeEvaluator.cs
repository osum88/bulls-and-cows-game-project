namespace bulls_and_cows_game_project.Services
{
    public class CodeEvaluator
    {


        public static (int bulls, int cows) EvaluateGuess(string secret, string guess)
        {
            
            if (guess.Length != 4 || !guess.All(char.IsDigit))
            {
                throw new ArgumentException("Guess must be a 4-digit number.", guess);
            }

            int bulls = 0;
            int cows = 0;
            List<char> bullsGuess = new List<char>();

            for (int i = 0; i < 4; i++)
            {
                if (guess[i] == secret[i])
                {
                    bulls++;
                    bullsGuess.Add(guess[i]);
                }
            }
            List<char> cowsGuess = new List<char>();
            for (int i = 0; i < 4; i++)
            {
                if (bullsGuess.Contains(guess[i]))
                {
                    continue;
                }
                else if (secret.Contains(guess[i]) && !cowsGuess.Contains(guess[i]))
                {
                    cows++;
                    cowsGuess.Add(guess[i]);
                }
            }

            return (bulls, cows);
        }
    }
}
