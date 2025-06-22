namespace bulls_and_cows_game_project.Models
{
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<GameSessionTag> GameSessionTags { get; set; }
    }
}
