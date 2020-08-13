using System;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;

namespace KMSGuildExtractor.Core.Parser
{
    internal static class GuildDataParser
    {

        public static GuildInfo FindGuildInHtml(HtmlDocument html, WorldID world)
        {
            try
            {
                HtmlNode rankNode = html.DocumentNode.SelectSingleNode("//table[@class=\"rank_table2\"]/tbody");

                foreach (HtmlNode item in rankNode.SelectNodes(".//tr[@class=\"\"]"))
                {
                    HtmlNode guildNode = item.SelectSingleNode("./td[2]/span/a");
                    string guildName = guildNode.InnerText.Trim();
                    string guildLink = guildNode.GetAttributeValue("href", string.Empty);
                    int guildLevel = ParseTool.GetDigitInString(item.SelectSingleNode("./td[3]").InnerText);
                    (int gid, WorldID wid) = ParseValueFromGuildLink(guildLink);

                    if (wid == world)
                    {
                        return new GuildInfo(guildName, wid, gid) { Level = guildLevel };
                    }
                }

                return null;
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Failed to parse guild search html", e);
            }
        }

        public static void AddGuildMembers(ref GuildInfo info, HtmlDocument html)
        {
            try
            {
                HtmlNode guildOrgNode = html.DocumentNode.SelectSingleNode("//table[@class=\"rank_table\"]/tbody");

                foreach (HtmlNode item in guildOrgNode.Descendants("tr"))
                {
                    string position = item.SelectSingleNode("./td[1]").InnerText.Trim();
                    string name = item.SelectSingleNode("./td[2]/dl/dt/a").InnerText.Trim();
                    info.Users.Add(new GuildUserInfo(name, info.World)
                    {
                        Position = ParseGuildPosition(position)
                    });
                }
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Faild to parse guild organization html", e);
            }
        }

        public static bool IsNextPageExist(HtmlDocument html)
        {
            try
            {
                HtmlNode node = html.DocumentNode.SelectSingleNode("//span[@class=\"cm_next\"]/a");
                return node.HasAttributes;
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Doesn't found next page button", e);
            }
        }

        private static GuildPosition ParseGuildPosition(string position) => position switch
        {
            "마스터" => GuildPosition.Owner,
            "부마스터" => GuildPosition.Staff,
            _ => GuildPosition.Member
        };


        private static (int gid, WorldID wid) ParseValueFromGuildLink(string url)
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
