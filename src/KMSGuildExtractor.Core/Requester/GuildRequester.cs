using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace KMSGuildExtractor.Core.Requester
{
    public class GuildRequester : BaseRequester
    {
        private const string GuildSearchLink = "https://maplestory.nexon.com/Ranking/World/Guild?n=";

        public async static Task<HtmlDocument> GetGuildSearchResultHtmlAsync(string name, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return null;
            }

            return await s_web.LoadFromWebAsync($"{GuildSearchLink}{name}", cancellationToken);
        }
    }
}
