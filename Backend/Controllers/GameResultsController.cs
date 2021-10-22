using Backend.Data;
using Backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    [Route("Endpoint1/[controller]")]
    [ApiController]
    public class GameResultsController : ControllerBase
    {
        private readonly IDistributedCache _cache;
        private readonly IConfiguration _configuration;
        private readonly DesafioContext _context;

        public GameResultsController(IDistributedCache cache, IConfiguration configuration, DesafioContext context)
        {
            _cache = cache;
            _context = context;
            _configuration = configuration;
        }

        // POST: Endpoint1/GameResults
        [HttpPost]
        public async Task<ActionResult> PostGameResultsAsync([FromBody] GameResult gameResult)
        {
            try
            {
                gameResult.Timestamp = DateTime.Now;

                // Peristir na memoria cache
                await SaveMemoryCache(gameResult);

                // Persiste no banco de dados
                _context.GameResult.Add(gameResult);

                if (gameResult.Player != null)
                    _context.Entry(gameResult.Player).State = EntityState.Unchanged;
                if (gameResult.Game != null)
                    _context.Entry(gameResult.Game).State = EntityState.Unchanged;

                /* Persistir no banco de dados*/
                await _context.SaveChangesAsync();

                return Ok($"Dados salvos com sucesso!");
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao salvar os dados, tente novamente. " + ex.Message);
            }
        }

        private async Task SaveMemoryCache(GameResult gameResult)
        {
            // Gera o objeto para salvar na memória
            var data = new { gameResult.Win, gameResult.PlayerId };

            List<dynamic> lista = new();

            // Obtém o que tem na memória
            var cacheString = await _cache.GetStringAsync("_Balances");

            if (!String.IsNullOrEmpty(cacheString))
            {
                var listCache = JsonConvert.DeserializeObject<List<dynamic>>(cacheString);
                lista.AddRange(listCache);
            }

            // Adiciona o novo conteúdo a lista
            lista.Add(data);

            // Serializa a nova lista para string
            var json = JsonConvert.SerializeObject(lista);

            var options = new DistributedCacheEntryOptions()
                .SetSlidingExpiration(TimeSpan.FromSeconds(Convert.ToDouble(_configuration["CacheTimeout:SlidingExpiration"])))
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(Convert.ToDouble(_configuration["CacheTimeout:AbsoluteExpiration"])));

            // Persistir na memória 
            await _cache.SetStringAsync("_Balances", json, options);
        }
    }
}
