using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KMSGuildExtractor.Core.Requester
{
    internal class UserDataRequester : BaseRequester
    {
        private const string UserDataUrl = "https://maple.gg/u/{0}";
        private const string UserSyncUrl = UserDataUrl + "/sync";

        public static async Task<SyncData> GetUserSyncData(string name)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(UserSyncUrl, name));
            using HttpResponseMessage response = await s_client.SendAsync(request);

            byte[] json = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<SyncData>(json.AsSpan());
        }

        public class SyncData
        {
            [JsonPropertyName("error")]
            public bool Error { get; set; }

            [JsonPropertyName("done")]
            public bool Done { get; set; }

            [JsonPropertyName("message")]
            public string Message { get; set; }

            [JsonPropertyName("interval")]
            public int Interval { get; set; }
        }
    }
}
