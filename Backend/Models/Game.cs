using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Game
    {
        [Key]
        public long GameId { get; set; }
        public string Name { get; set; }

        public ICollection<GameResult> GameResults { get; set; } 
    }
}
