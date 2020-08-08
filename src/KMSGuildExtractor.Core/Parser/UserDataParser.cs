using System;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;

namespace KMSGuildExtractor.Core.Parser
{
    internal class UserDataParser
    {
        public static bool TryGetUserDetail(ref UserInfo user, HtmlDocument html)
        {
            try
            {
                string lastUpdatedRaw = html.DocumentNode.SelectSingleNode("//div[@class=\"mb-1 text-white\"]/span").InnerText;
                int lastUpdated = ParseTool.GetDigitInString(lastUpdatedRaw);
                DateTime subract = DateTime.Now.Subtract(TimeSpan.FromDays(lastUpdated));

                HtmlNodeCollection profile = html.DocumentNode.SelectNodes("//ul[@class=\"user-summary-list\"]/li");
                string levelRaw = profile[0].InnerText;
                int level = ParseTool.GetDigitInString(levelRaw);

                string job = profile[1].InnerText.Trim();

                string popularityRaw = profile[2].InnerText;
                int popularity = ParseTool.GetDigitInString(popularityRaw);

                HtmlNodeCollection datas = html.DocumentNode.SelectNodes("//section[@class=\"box user-summary-box\"]");

                string dojangFloorRaw = datas[0].SelectSingleNode(".//h1").InnerText;
                int dojangFloor = ParseTool.GetDigitInString(dojangFloorRaw);

                string unionRaw = datas[2].SelectSingleNode(".//span[@class=\"user-summary-level\"]").InnerText;
                int union = ParseTool.GetDigitInString(unionRaw);

                user = new UserInfo(user.Name, user.World)
                {
                    LastUpdated = new DateTime(subract.Year, subract.Month, subract.Day),
                    Level = level,
                    Job = job,
                    Popularity = popularity,
                    DojangFloor = dojangFloor,
                    UnionLevel = union
                };
                return true;
            }
            catch (NullReferenceException e) when (html.GetElementbyId("app").SelectSingleNode(".//img[@alt=\"검색결과 없음\"]") != null)
            {
                throw new ParseException("User could not be found.", e);
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Faild to parse user detail html", e);
            }
        }
    }
}
