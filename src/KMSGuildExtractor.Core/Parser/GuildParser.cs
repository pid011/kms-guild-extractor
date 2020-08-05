using System;
using System.Globalization;
using System.Text.RegularExpressions;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;

namespace KMSGuildExtractor.Core.Parser
{
public static class GuildParser
{

    /// <summary>
    /// 월드와 일치하는 길드를 찾는다.
    /// </summary>
    /// <param name="html">검색할 html문서</param>
    /// <param name="world">검색할 길드의 월드</param>
    /// <returns>만약 일치하는 길드가 없거나 잘못된 html문서면 null을 반환</returns>
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
                    return new GuildInfo(guildName, wid, gid) {
                        Level = guildLevel
                    };
                }
            }

            return null;
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public static WeeklyGuildReputationInfo GetWeeklyReputation(HtmlDocument html)
    {

        try
        {
            HtmlNode infoNode = html.DocumentNode.SelectSingleNode("//div[@class=\"char_info_top\"]");
            HtmlNodeCollection weeklyRankNode = infoNode.SelectNodes("./div[@class=\"char_info\"]/dl");

            string weeklyOverallRankRaw = weeklyRankNode[0].SelectSingleNode("./dd[@class=\"num\"]").InnerText;
            int weeklyOverallRank = ParseTool.GetDigitInString(weeklyOverallRankRaw);

            string weeklyWorldRankRaw = weeklyRankNode[1].SelectSingleNode("./dd[@class=\"num\"]").InnerText;
            int weeklyWorldRank = ParseTool.GetDigitInString(weeklyWorldRankRaw);

            string weeklyScoreRaw = infoNode.SelectNodes(".//ul[@class=\"info_tb_left_list\"]/li")[2].GetDirectInnerText();
            IFormatProvider provider = CultureInfo.CreateSpecificCulture("ko-KR");
            int weeklyScore = int.TryParse(weeklyScoreRaw, NumberStyles.Number | NumberStyles.Integer, provider, out int result) ? result : 0;

            return new WeeklyGuildReputationInfo
            {
                Score = weeklyScore,
                OverallRank = weeklyOverallRank,
                WorldRank = weeklyWorldRank
            };
        }
        catch (NullReferenceException)
        {
            return null;
        }
    }

    public static bool TryAddGuildOrganization(ref GuildInfo info, HtmlDocument html)
    {
        try
        {
            HtmlNode guildOrgNode = html.DocumentNode.SelectSingleNode("//table[@class=\"rank_table\"]/tbody");

            foreach (HtmlNode item in guildOrgNode.Descendants("tr"))
            {
                //WriteLine(item.OuterHtml);
                string position = item.SelectSingleNode("./td[1]").InnerText.Trim();
                string name = item.SelectSingleNode("./td[2]/dl/dt/a").InnerText.Trim();
                info.Users.Add(new GuildUserInfo(name, info.World)
                {
                    Position = ParseGuildPosition(position)
                });
            }

            return true;
        }
        catch (NullReferenceException)
        {
            return false;
        }
    }

    public static bool IsNextPageExist(HtmlDocument html)
    {
        try
        {
            HtmlNode node = html.DocumentNode.SelectSingleNode("//span[@class=\"cm_next\"]/a");
            return node.HasAttributes;
        }
        catch (NullReferenceException)
        {
            return false;
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
