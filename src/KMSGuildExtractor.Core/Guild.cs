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
        /// <summary>
        /// 길드를 검색합니다.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="wid"></param>
        /// <param name="cancellation"></param>
        /// <returns>길드가 존재하면 <see cref="GuildInfo"/>를 반환, 존재하지 않거나 html파싱에 실패했다면 null을 반환</returns>
        public static async Task<GuildInfo> SearchGuildAsync(string name, WorldID wid, CancellationToken cancellation)
        {
            HtmlDocument html = await GuildDataRequester.GetGuildSearchResultHtmlAsync(name, cancellation);

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
                next = GuildDataParser.TryAddGuildMembers(ref result, html) && GuildDataParser.IsNextPageExist(html);
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
