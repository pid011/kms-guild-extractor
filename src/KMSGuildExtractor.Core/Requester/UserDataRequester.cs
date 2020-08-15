using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace KMSGuildExtractor.Core.Requester
{
    internal class UserDataRequester : BaseRequester
    {
        private const string UserDataUrl = "https://maple.gg/u/{0}";
        private const string UserSyncUrl = UserDataUrl + "/sync";

        public static async Task<SyncData> GetUserSyncDataAsync(string name, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(name));
            }

            var request = new HttpRequestMessage(HttpMethod.Get, string.Format(UserSyncUrl, name));
            using HttpResponseMessage response = await s_client.SendAsync(request, cancellation);

            if (cancellation.IsCancellationRequested)
            {
                return null;
            }
            byte[] json = await response.Content.ReadAsByteArrayAsync();
            return JsonSerializer.Deserialize<SyncData>(json.AsSpan());
        }

        public static async Task<HtmlDocument> GetUserDataHtmlAsync(string name, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("User name cannot be null or empty.", nameof(name));
            }

            return await s_web.LoadFromWebAsync(string.Format(UserDataUrl, name), cancellation);
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
