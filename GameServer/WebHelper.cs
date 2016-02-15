using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;

namespace Game
{
    public class WebHelper
    {
        public static async Task<string> RequestPlayerInfo(string userKey)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                   { "username", userKey }
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://firstapp.dongjun.in:8000/app/getGameMoney/", content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }

        public static async Task<string> UpdatePlayerMoney(string userKey, int money)
        {
            using (var client = new HttpClient())
            {
                var values = new Dictionary<string, string>
                {
                   { "username", userKey },
                   { "game_money", money.ToString() }
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("http://firstapp.dongjun.in:8000/app/addGameMoney/", content);

                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
        }
    }
}
