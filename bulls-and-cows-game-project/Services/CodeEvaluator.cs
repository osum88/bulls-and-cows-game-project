namespace bulls_and_cows_game_project.Services
{
    public class CodeEvaluator
    {
        public static string GenerateSecretCode(int length = 4)
        {
            var rnd = new Random();
            var digits = "0123456789".ToCharArray();

            for (int i = digits.Length - 1; i > 0; i--)
            {
                int j = rnd.Next(i + 1);
                (digits[i], digits[j]) = (digits[j], digits[i]);
            }

            return new string(digits, 0, length);
        }

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
