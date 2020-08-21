using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace KMSGuildExtractor.Core.Requester
{
    internal class GitHubRequester : BaseRequester
    {
        private const string LatestReleaseLink = "https://api.github.com/repos/pid011/kms-guild-extractor/releases/latest";

        public static async Task<ReleaseData?> GetLastReleaseAsync(CancellationToken cancellationToken = default)
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, LatestReleaseLink);
            request.Headers.Add("User-Agent", "kms-guild-extractor");
            using HttpResponseMessage response = await s_client.SendAsync(request, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                return null;
            }

            using Stream jsonStream = await response.Content.ReadAsStreamAsync();
            return await JsonSerializer.DeserializeAsync<ReleaseData>(jsonStream, cancellationToken: cancellationToken);
        }

        public class ReleaseData
        {
            [JsonPropertyName("html_url")]
            public string? Url { get; set; }

            [JsonPropertyName("tag_name")]
            public string? TagName { get; set; }
        }
    }
}
