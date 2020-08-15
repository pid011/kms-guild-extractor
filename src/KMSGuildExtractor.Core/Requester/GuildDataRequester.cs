using System;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

namespace KMSGuildExtractor.Core.Requester
{
    internal class GuildDataRequester : BaseRequester
    {
        private const string GuildSearchLink = "https://maplestory.nexon.com/Ranking/World/Guild?n={0}";
        private const string GuildOrganizationLink = "https://maplestory.nexon.com/Common/Guild?gid={0}&wid={1}&orderby=1&page={2}";

        public static async Task<HtmlDocument> GetGuildSearchHtmlAsync(string name, CancellationToken cancellation = default)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Guild name cannot be null or empty.", nameof(name));
            }

            return await s_web.LoadFromWebAsync(string.Format(GuildSearchLink, name), cancellation);
        }

        public static async Task<HtmlDocument> GetGuildOrganizationHtmlAsync(int gid, WorldID wid, CancellationToken cancellation = default, int page = 1)
        {
            if (gid < 0)
            {
                throw new ArgumentException("Guild ID cannot be negative.", nameof(gid));
            }

            return await s_web.LoadFromWebAsync(string.Format(GuildOrganizationLink, gid, (int)wid, page), cancellation);
        }
    }
}
