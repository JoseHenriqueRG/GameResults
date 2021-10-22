using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Models
{
    public class Player
    {
        [Key]
        public long PlayerId { get; set; }
        public string Name { get; set; }
        public long Balance { get; set; }
        public DateTime LastUpdateDate { get; set; }

        public ICollection<GameResult> GameResults { get; set; }
    }
}
