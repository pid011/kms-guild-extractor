using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Requester;

namespace KMSGuildExtractor.Core
{
    public class Guild : IGuild
    {
        public int GuildID { get; }

        public WorldID World { get; }

        public string Name { get; }

        public int? Level { get; private set; }

        public IList<(GuildPosition position, User data)> Members { get; private set; }

        public Guild(string name, WorldID world, int guildID)
        {
            Name = name;
            World = world;
            GuildID = guildID;
        }

        public static async Task<Guild> SearchAsync(string name, WorldID wid, CancellationToken cancellation = default)
        {
            HtmlDocument html = await GuildDataRequester.GetGuildSearchHtmlAsync(name, cancellation);

            return FindGuildInHtml(html, wid);
        }

        public static bool IsValidGuildName(string guildName)
        {
            if (!Regex.IsMatch(guildName, "^[0-9a-zA-Z가-힣]*$")) // 특수문자 입력 금지
            {
                return false;
            }

            float count = guildName.ToCharArray()
                                   .Sum(ch => Regex.IsMatch(ch.ToString(), "[0-9a-zA-Z]") ? 0.5f : 1f);

            return count >= 2 && count <= 6;
        }

        public async Task LoadGuildMembersAsync(CancellationToken cancellation = default)
        {
            List<(GuildPosition, User)> members = new List<(GuildPosition, User)>(200);
            int i = 1;

            while (true)
            {
                HtmlDocument html = await GuildDataRequester.GetGuildOrganizationHtmlAsync(GuildID, World, cancellation, i);
                members.AddRange(GetGuildMembersInHtml(html));

                if (IsNextPageExist(html))
                {
                    i++;
                    await Task.Delay(750, cancellation);
                }
                else
                {
                    Members = members;
                    return;
                }
            }
        }

        private static Guild FindGuildInHtml(HtmlDocument html, WorldID world)
        {
            try
            {
                HtmlNode rankNode = html.DocumentNode.SelectSingleNode("//table[@class=\"rank_table2\"]/tbody");

                if (rankNode is null)
                {
                    return null;
                }

                foreach (HtmlNode item in rankNode.SelectNodes("./tr"))
                {
                    HtmlNode guildNode = item.SelectSingleNode("./td[2]/span//a");
                    string guildName = guildNode.InnerText.Trim();
                    string guildLink = guildNode.GetAttributeValue("href", string.Empty);
                    int guildLevel = item.SelectSingleNode("./td[3]").InnerText.GetDigit() ?? 0;
                    (int gid, WorldID wid) = ParseValueFromGuildLink(guildLink);

                    if (wid == world)
                    {
                        return new Guild(guildName, wid, gid) { Level = guildLevel };
                    }
                }

                return null;
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Failed to parse guild search html", e);
            }

            static (int gid, WorldID wid) ParseValueFromGuildLink(string url)
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

        private IEnumerable<(GuildPosition, User)> GetGuildMembersInHtml(HtmlDocument html)
        {
            List<(GuildPosition, User)> members = new List<(GuildPosition, User)>(30);
            try
            {
                HtmlNode guildOrgNode = html.DocumentNode.SelectSingleNode("//table[@class=\"rank_table\"]/tbody");

                foreach (HtmlNode item in guildOrgNode.Descendants("tr"))
                {
                    string position = item.SelectSingleNode("./td[1]").InnerText.Trim();
                    string name = item.SelectSingleNode("./td[2]/dl/dt/a").InnerText.Trim();
                    members.Add((ParseGuildPosition(position), new User(name, World)));
                }

                return members;
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Faild to parse guild organization html", e);
            }

            static GuildPosition ParseGuildPosition(string position) => position switch
            {
                "마스터" => GuildPosition.Owner,
                "부마스터" => GuildPosition.Staff,
                _ => GuildPosition.Member
            };
        }

        private bool IsNextPageExist(HtmlDocument html)
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
    }
}
