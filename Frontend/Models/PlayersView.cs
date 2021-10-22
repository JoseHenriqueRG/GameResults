using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Frontend.Models
{
    public class PlayerView
    {
        public long PlayerId { get; set; }
        [Display(Name = "Nome")]
        public string Name { get; set; }
    }
}
