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
    public partial class Guild : IGuild
    {
        public int GuildID { get; }
        public WorldID World { get; }
        public string Name { get; }
        public int? Level { get; private set; }
        public IReadOnlyList<GuildMember> Members { get; private set; }

        public Guild(string name, WorldID world, int guildID)
        {
            Name = name;
            World = world;
            GuildID = guildID;
            Members = new List<GuildMember>();
        }

        public static async Task<Guild> SearchAsync(string name, WorldID wid, CancellationToken cancellation = default)
        {
            HtmlDocument html = await GuildDataRequester.RequestGuildSearchHtmlAsync(name, cancellation);
            return FindGuildInHtml(html, wid);
        }

        public static bool IsValidGuildName(string guildName)
        {
            if (!GuildNameChecker().IsMatch(guildName)) // 특수문자 입력 금지
            {
                return false;
            }

            float count = guildName.ToCharArray()
                                   .Sum(ch => GuildNameAlphabetChecker().IsMatch(ch.ToString()) ? 0.5f : 1f);

            return count >= 2 && count <= 6;
        }

        public async Task LoadGuildMembersAsync(CancellationToken cancellation = default)
        {
            var members = new List<GuildMember>(200);

            int i = 1;
            while (true)
            {
                HtmlDocument html = await GuildDataRequester.RequestGuildHtmlAsync(GuildID, World, i, cancellation);
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
                HtmlNode rankNode = html.DocumentNode.SelectSingleNode(".//table[@class=\"rank_table2\"]/tbody");

                if (rankNode is null)
                {
                    return null;
                }

                foreach (HtmlNode item in rankNode.SelectNodes("./tr"))
                {
                    HtmlNode guildNode = item.SelectSingleNode("./td[2]/span//a");
                    string guildName = guildNode.InnerText.Trim();
                    string guildLink = guildNode.GetAttributeValue("href", string.Empty);

                    int guildLevel = item.SelectSingleNode("./td[3]").InnerText.ParseInt() ?? 0;
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
                MatchCollection matches = GuildLinkParser().Matches(url);
                (int gid, WorldID wid) = (-1, WorldID.Unknown);

                foreach (Match match in matches.Cast<Match>())
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

        private IEnumerable<GuildMember> GetGuildMembersInHtml(HtmlDocument html)
        {
            try
            {
                var members = new List<GuildMember>(50);
                HtmlNode guildOrgNode = html.DocumentNode.SelectSingleNode(".//table[@class=\"rank_table\"]/tbody");

                foreach (HtmlNode item in guildOrgNode.Descendants("tr"))
                {
                    string position = item.SelectSingleNode("./td[1]").InnerText.Trim();
                    string name = item.SelectSingleNode("./td[2]/dl/dt/a").InnerText.Trim();
                    var member = new GuildMember(ParseGuildPosition(position), new User(name, World));
                    members.Add(member);
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

        private static bool IsNextPageExist(HtmlDocument html)
        {
            try
            {
                HtmlNode node = html.DocumentNode.SelectSingleNode(".//span[@class=\"cm_next\"]/a");
                return node.HasAttributes;
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Doesn't found next page button", e);
            }
        }

        [GeneratedRegex("[0-9a-zA-Z]")]
        private static partial Regex GuildNameAlphabetChecker();
        [GeneratedRegex("^[0-9a-zA-Z가-힣]*$")]
        private static partial Regex GuildNameChecker();
        [GeneratedRegex("(?:\\?|&|;)([^=]+)=([^&|;]+)")]
        private static partial Regex GuildLinkParser();
    }
}
