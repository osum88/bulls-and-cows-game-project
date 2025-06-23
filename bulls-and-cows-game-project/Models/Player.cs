// Player.cs
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;

namespace bulls_and_cows_game_project.Models
{
    public class Player : IdentityUser
    {
        public ICollection<GameSession> GameSessions { get; set; }
    }
}
