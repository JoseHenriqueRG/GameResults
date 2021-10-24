using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Backend.Models;
using System.Linq;

namespace Backend
{
    public class WorkerService : BackgroundService
    {
        private readonly ILogger<WorkerService> _logger;
        private readonly IDistributedCache _cache;

        public WorkerService(ILogger<WorkerService> logger, IDistributedCache cache)
        {
            _logger = logger;
            _cache = cache;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await DeserializeGameResult();

                var _time = Convert.ToInt32(Startup.StaticConfig["CheckUpdateTime"]);

                await Task.Delay(_time, stoppingToken);
            }
        }

        private async Task DeserializeGameResult()
        {
            var cacheString = await _cache.GetStringAsync("_Balances");

            if (!String.IsNullOrEmpty(cacheString))
            {
                try
                {
                    // Deserializa o objeto
                    var arrayObj = JsonConvert.DeserializeObject<object[]>(cacheString);

                    // Cria a lista de GameResult
                    List<GameResult> lista = new List<GameResult>();
                    foreach (dynamic item in arrayObj)
                    {
                        GameResult gameResult = new GameResult
                        {
                            Win = item.Win.Value,
                            PlayerId = item.PlayerId
                        };

                        lista.Add(gameResult);
                    }

                    foreach (long playerId in lista.Select(g => g.PlayerId).Distinct())
                    {
                        long balance = lista.Where(g => g.PlayerId == playerId).Sum(g => g.Win);

                        bool valid = await PersistDatabase(balance, playerId);

                        // Se foi salvo remove da lista
                        if (valid)
                        {
                            lista = lista.Where(g => g.PlayerId != playerId).ToList();
                        }
                    }

                    /* 
                     * ** Lista.Count <= 0 significa que todo cache foi salvo. **
                     * ** Se contém dados na lista limpa o cache e salva os dados 
                     * que não foram gravados no banco de dados no cache novamente. **
                     */
                    if (lista.Count > 0)
                    {
                        await _cache.RemoveAsync("_Balances");

                        // Serializa a lista para string
                        var json = JsonConvert.SerializeObject(lista);

                        var options = new DistributedCacheEntryOptions()
                            .SetSlidingExpiration(TimeSpan.FromSeconds(Convert.ToDouble(Startup.StaticConfig["CacheTimeout:SlidingExpiration"])))
                            .SetAbsoluteExpiration(TimeSpan.FromMinutes(Convert.ToDouble(Startup.StaticConfig["CacheTimeout:AbsoluteExpiration"])));

                        // Persistir na memória 
                        await _cache.SetStringAsync("_Balances", json, options);
                    }
                    else
                    {
                        await _cache.RemoveAsync("_Balances");
                    }                    
                }
                catch (Exception exception)
                {
                    _logger.LogCritical(exception, "FATAL ERROR: {Message}", exception.Message);
                }
            }
        }

        private Task<bool> PersistDatabase(long balance, long playerId)
        {
            using var conn = new SqlConnection(Startup.StaticConfig["ConnectionStrings:DesafioContext"]);
            try
            {
                conn.Open();
                SqlCommand cmd2 = new SqlCommand("UPDATE P SET P.LASTUPDATEDATE = @Datetime, P.BALANCE = A.BALANCE " +
                    "FROM PLAYER AS P INNER JOIN ( SELECT PLAYERID, SUM(BALANCE + @Balance) BALANCE FROM PLAYER WHERE PLAYERID = @PlayerId " +
                    "GROUP BY PLAYERID) A ON A.PLAYERID = P.PLAYERID WHERE P.PLAYERID = @PlayerId"
                    , conn);
                cmd2.Parameters.AddWithValue("@Balance", balance);
                cmd2.Parameters.AddWithValue("@PlayerId", playerId);
                cmd2.Parameters.AddWithValue("@Datetime", DateTime.Now);

                using (SqlCommand command = cmd2)
                {
                    using SqlDataReader reader = command.ExecuteReader();
                }
                conn.Close();
                return Task.FromResult(true);
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}", exception.Message);
                return Task.FromResult(false);
            }
        }
    }
}
