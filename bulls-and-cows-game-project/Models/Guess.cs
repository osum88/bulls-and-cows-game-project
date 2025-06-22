namespace bulls_and_cows_game_project.Models
{
    public class Guess
    {
        public int Id { get; set; }
        public int GameSessionId { get; set; }
        public GameSession GameSession { get; set; }

        public string GuessCode { get; set; }
        public int CorrectPositionCount { get; set; }
        public int WrongPositionCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
