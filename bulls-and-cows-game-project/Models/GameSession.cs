namespace bulls_and_cows_game_project.Models
{
    public class GameSession
    {
        public int Id { get; set; }

        public string PlayerId { get; set; } 
        public Player Player { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string TargetCode { get; set; }
        public bool IsSolved { get; set; }
        public int TotalGuesses { get; set; }

        public ICollection<Guess> Guesses { get; set; }
        public ICollection<GameSessionTag> GameSessionTags { get; set; }
    }

}
