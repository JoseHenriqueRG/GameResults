using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class LeaderboardView
    {
        public long PlayerId { get; set; }
        public string Name { get; set; }
        public long Balance { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
