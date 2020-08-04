using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;

namespace KMSGuildExtractor.Core.Parser
{
    public static class GuildParser
    {

        /// <summary>
        /// 월드와 일치하는 길드의 길드아이디를 찾는다.
        /// </summary>
        /// <param name="html">검색할 html문서</param>
        /// <param name="world">검색할 길드의 월드</param>
        /// <param name="guildID">길드아이디</param>
        /// <returns>만약 일치하는 길드가 없으면 false, 아니면 true를 반환</returns>
        public static bool TryGetGuildID(HtmlDocument html, WorldID world, out int guildID)
        {
            guildID = -1;

            if (html == null)
            {
                return false;
            }

            HtmlNode rankNode = html.DocumentNode.SelectSingleNode("//table[@class=\"rank_table2\"]/tbody");

            if (rankNode == null)
            {
                return false;
            }

            foreach (HtmlNode item in rankNode.SelectNodes("//tr[@class=\"\"]"))
            {
                HtmlNode guildNode = item.SelectSingleNode("td[2]/span/a");
                string guildLink = guildNode.GetAttributeValue("href", string.Empty);
                (int gid, WorldID wid) = GetValueFromGuildLink(guildLink);

                if (wid == world)
                {
                    guildID = gid;
                    return true;
                }
                continue;
            }

            return false;
        }


        private static (int gid, WorldID wid) GetValueFromGuildLink(string url)
        {
            MatchCollection matches = Regex.Matches(url, @"(?:\?|&|;)([^=]+)=([^&|;]+)");
            (int gid, WorldID wid) = (-1, WorldID.Unknown);

            foreach (Match match in matches)
            {
                string[] param = match.Value.Split('=');
                int tmp = int.Parse(param[1]);
                switch (param[0].Remove(0, 1))
                {
                    case "wid":
                        wid = Enum.IsDefined(typeof(WorldID), tmp) ? (WorldID)tmp : WorldID.Unknown;
                        break;

                    case "gid":
                        gid = tmp;
                        break;
                }
            }

            return (gid, wid);
        }
    }
}
