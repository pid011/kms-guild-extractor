using System;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Requester;

namespace KMSGuildExtractor.Core
{
    public class User : IUser
    {
        public WorldID World { get; }

        public string Name { get; private set; }

        public int? Level { get; private set; }

        public DateTime LastUpdated { get; private set; }

        public string Job { get; private set; }

        public int? Popularity { get; private set; }

        public int? DojangFloor { get; private set; }

        public int? UnionLevel { get; private set; }

        public User(string name, WorldID world)
        {
            Name = name;
            World = world;
        }

        public async Task LoadUserDetailAsync(CancellationToken cancellation = default)
        {
            HtmlDocument html = await UserDataRequester.GetUserDataHtmlAsync(Name, cancellation);
            SetUserDetail(html);
        }

        public async Task RequestSyncAsync(CancellationToken cancellation = default)
        {
            while (true)
            {
                UserDataRequester.SyncData data = await UserDataRequester.GetUserSyncDataAsync(Name, cancellation);

                if (data.Error)
                {
                    throw new UserSyncException(data.Message);
                }
                if (data.Done)
                {
                    return;
                }
                await Task.Delay(data.Interval, cancellation);
            }
        }

        private void SetUserDetail(HtmlDocument html)
        {
            try
            {
                string lastUpdatedRaw = html.DocumentNode.SelectSingleNode("//div[@class=\"mb-1 text-white\"]/span").InnerText;
                int lastUpdated = lastUpdatedRaw.GetDigit();
                DateTime subract = DateTime.Now.Subtract(TimeSpan.FromDays(lastUpdated));

                HtmlNodeCollection profile = html.DocumentNode.SelectNodes("//ul[@class=\"user-summary-list\"]/li");
                string levelRaw = profile[0].InnerText;
                int level = levelRaw.GetDigit();

                string job = profile[1].InnerText.Trim();

                string popularityRaw = profile[2].InnerText;
                int popularity = popularityRaw.GetDigit();

                HtmlNodeCollection datas = html.DocumentNode.SelectNodes("//section[@class=\"box user-summary-box\"]");

                string dojangFloorRaw = datas[0].SelectSingleNode(".//h1").InnerText;
                int dojangFloor = dojangFloorRaw.GetDigit();

                string unionRaw = datas[2].SelectSingleNode(".//span[@class=\"user-summary-level\"]").InnerText;
                int union = unionRaw.GetDigit();

                LastUpdated = new DateTime(subract.Year, subract.Month, subract.Day);
                Level = level;
                Job = job;
                Popularity = popularity;
                DojangFloor = dojangFloor;
                UnionLevel = union;
            }
            catch (NullReferenceException) when (html.GetElementbyId("app").SelectSingleNode(".//img[@alt=\"검색결과 없음\"]") != null)
            {
                throw new UserNotFoundException();
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Faild to parse user detail html", e);
            }
        }
    }
}
