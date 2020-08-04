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
        public static async Task<(bool exist, int guildID)> TryFindGuildID(string name, WorldID world,
                                                                           CancellationToken cancellationToken)
        {
            (bool exist, int gid) result = (false, -1);

            HtmlDocument html = await GuildRequester.GetGuildSearchResultHtmlAsync(name, cancellationToken);

            if (!cancellationToken.IsCancellationRequested)
            {
                result.exist = GuildParser.TryGetGuildID(html, world, out result.gid);
            }

            return result;
        }
    }
}
