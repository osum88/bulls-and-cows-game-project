using Azure;

namespace bulls_and_cows_game_project.Models
{
    public class GameSessionTag
    {
        public int GameSessionId { get; set; }
        public GameSession GameSession { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
