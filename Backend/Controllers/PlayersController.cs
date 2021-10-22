using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Data;
using Backend.Models;

namespace Backend.Controllers
{
    [Route("Players")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly DesafioContext _context;

        public PlayersController(DesafioContext context)
        {
            _context = context;
        }

        // GET: Players/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Player>> GetPlayer(long id)
        {
            var player = await _context.Player.FindAsync(id);

            if (player == null)
            {
                return NotFound();
            }

            return player;
        }

        // POST: Players
        [HttpPost]
        public async Task<ActionResult<Player>> PostPlayer(Player player)
        {
            player.LastUpdateDate = DateTime.Now;
            _context.Player.Add(player);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPlayer", new { id = player.PlayerId }, player);
        }
    }
}
