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
            _logger.LogDebug($"GracePeriodManagerService is starting.");

            stoppingToken.Register(() =>
                _logger.LogDebug($" GracePeriod background task is stopping."));

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogDebug($"GracePeriod task doing background work.");

                await DeserializeGameResult();

                var _time = Convert.ToInt32(Startup.StaticConfig["CheckUpdateTime"]);

                await Task.Delay(_time, stoppingToken);
            }

            _logger.LogDebug($"GracePeriod background task is stopping.");
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

                        await PersistDatabase(balance, playerId);
                    }

                    await _cache.RemoveAsync("_Balances");
                }
                catch (Exception exception)
                {
                    _logger.LogCritical(exception, "FATAL ERROR: {Message}", exception.Message);
                }
            }
        }

        private Task PersistDatabase(long balance, long playerId)
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
            }
            catch (Exception exception)
            {
                _logger.LogCritical(exception, "FATAL ERROR: Database connections could not be opened: {Message}", exception.Message);
            }

            return Task.CompletedTask;
        }
    }
}
