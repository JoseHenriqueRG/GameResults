using Frontend.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Frontend.Service
{
    public static class BackendService
    {
        private static readonly HttpClient _client = new HttpClient();

        public static async Task<List<LeaderboardView>> GetBalanceAsync()
        {
            List<LeaderboardView> leaderboard = new();

            var url = Startup.StaticConfig["Endpoint:Endpoint2"] + "Leaderboards";

            HttpResponseMessage response = await _client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                leaderboard = JsonConvert.DeserializeObject<List<LeaderboardView>>(jsonString);
            }

            return leaderboard;
        }

        public static async Task<PlayerView> GetPlayerAsync(long id)
        {
            PlayerView Player = new();

            var url = Startup.StaticConfig["Endpoint:Players"] + id;

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Player = JsonConvert.DeserializeObject<PlayerView>(jsonString);
            }

            return Player;
        }

        public static async Task<PlayerView> PostPlayerAsync(PlayerView item)
        {
            PlayerView Player = new();

            var url = Startup.StaticConfig["Endpoint:Players"];

            var data = JsonConvert.SerializeObject(item);

            HttpResponseMessage response =
                await _client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));

            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Player = JsonConvert.DeserializeObject<PlayerView>(jsonString);
            }

            return Player;
        }

        public static async Task<List<GameView>> GetGamesAsync()
        {
            List<GameView> Games = new();

            var url = Startup.StaticConfig["Endpoint:Games"];

            HttpResponseMessage response = await _client.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var jsonString = await response.Content.ReadAsStringAsync();
                Games = JsonConvert.DeserializeObject<List<GameView>>(jsonString);
            }

            return Games;
        }

        public static async Task<bool> PostGameResultAsync(GameResultView item)
        {
            var url = Startup.StaticConfig["Endpoint:Endpoint1"] + "GameResults";

            var data = JsonConvert.SerializeObject(item);

            HttpResponseMessage response =
                await _client.PostAsync(url, new StringContent(data, Encoding.UTF8, "application/json"));

            if (response.StatusCode == HttpStatusCode.OK)
            {
                return true;
            }

            return false;
        }
    }
}
