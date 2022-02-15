using System;
using System.Net.Http;
using System.Text;
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
                throw new ArgumentException("User name cannot be empty.", nameof(name));
            }

            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, string.Format(UserSyncUrl, name));
                using HttpResponseMessage response = await s_client.SendAsync(request, cancellation);

                var jsonByte = await response.Content.ReadAsByteArrayAsync(cancellation);

                try
                {
                    return JsonSerializer.Deserialize<SyncData>(jsonByte.AsSpan());
                }
                catch (JsonException e)
                {
                    throw new UserSyncException(name, "Faild to parse user sync json data.", e, Encoding.UTF8.GetString(jsonByte));
                }
            }
            catch (HttpRequestException e)
            {
                throw new UserSyncException(name, "Faild to request user sync data", e);
            }
            catch (Exception e)
            {
                throw new UserSyncException(name, "Faild to get user sync data", e);
            }
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
            public bool? Error { get; init; }

            [JsonPropertyName("done")]
            public bool? Done { get; init; }

            [JsonPropertyName("message")]
            public string Message { get; init; }

            [JsonPropertyName("interval")]
            public int? Interval { get; init; }
        }
    }
}
