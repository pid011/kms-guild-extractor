using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;

namespace KMSGuildExtractor.Core.Requester
{
    internal class GuildDataRequester : BaseRequester
    {
        private const string GuildSearchLink = "https://maplestory.nexon.com/Ranking/World/Guild?n={0}";
        private const string GuildOrganizationLink = "https://maplestory.nexon.com/Common/Guild?gid={0}&wid={1}&orderby=1&page={2}";

        public static async Task<HtmlDocument> GetGuildSearchResultHtmlAsync(string name, CancellationToken cancel)
        {
            return string.IsNullOrWhiteSpace(name)
                ? null
                : await s_web.LoadFromWebAsync(string.Format(GuildSearchLink, name), cancel);
        }

        public static async Task<HtmlDocument> GetGuildOrganizationHtmlAsync(int gid, WorldID wid, CancellationToken cancel, int page = 1)
        {
            return gid < 0
                ? null
                : await s_web.LoadFromWebAsync(string.Format(GuildOrganizationLink, gid, (int)wid, page), cancel);
        }
    }
}
