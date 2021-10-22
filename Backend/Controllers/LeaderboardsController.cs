using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("Endpoint2/[controller]")]
    [ApiController]
    public class LeaderboardsController : ControllerBase
    {
        private readonly DesafioContext _context;

        public LeaderboardsController(DesafioContext context)
        {
            _context = context;
        }

        // GET: Endpoint2/Leaderboards
        [HttpGet]
        public async Task<ActionResult<List<Player>>> GetLeaderboards()
        {
            return await _context.Player.OrderByDescending(p => p.Balance).Take(100).ToListAsync();
        }
    }
}
