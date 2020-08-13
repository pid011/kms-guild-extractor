using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;
using KMSGuildExtractor.Core.Parser;
using KMSGuildExtractor.Core.Requester;

namespace KMSGuildExtractor.Core
{
    public static class Guild
    {
        public static async Task<GuildInfo> SearchGuildAsync(string name, WorldID wid, CancellationToken cancellation)
        {
            HtmlDocument html = await GuildDataRequester.GetGuildSearchHtmlAsync(name, cancellation);

            return cancellation.IsCancellationRequested
                ? null
                : GuildDataParser.FindGuildInHtml(html, wid);
        }

        public static async Task<GuildInfo> GetGuildDetailAsync(GuildInfo info, CancellationToken cancellation)
        {
            int i = 1;
            bool next = true;

            GuildInfo result = new GuildInfo(info.Name, info.World, info.GuildID);

            while (!cancellation.IsCancellationRequested && next)
            {
                HtmlDocument html = await GuildDataRequester.GetGuildOrganizationHtmlAsync(info.GuildID, info.World, cancellation, i);
                if (cancellation.IsCancellationRequested)
                {
                    break;
                }

                GuildDataParser.AddGuildMembers(ref result, html);
                next = GuildDataParser.IsNextPageExist(html);
                i++;

                if (next)
                {
                    await Task.Delay(750, cancellation);
                }
            }

            return result;
        }
    }
}
