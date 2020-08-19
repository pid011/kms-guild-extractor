using System;
using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Requester;

namespace KMSGuildExtractor.Core
{
    public class User : IUser
    {
        /// <summary>
        /// 플레이어가 속한 월드
        /// </summary>
        public WorldID World { get; }

        /// <summary>
        /// 플레이어의 이름
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// 플레이어의 레벨
        /// </summary>
        public int Level { get; private set; }

        /// <summary>
        /// 플레이어가 마지막으로 활동한 날짜. 현재 날짜에서 마지막 활동일을 뺀 일 수를 반환한다.<br/>
        /// 만약 가져올 수 없다면 null을 반환한다.
        /// </summary>
        public int? LastUpdated { get; private set; }

        /// <summary>
        /// 플레이어의 직업
        /// </summary>
        public string Job { get; private set; }

        /// <summary>
        /// 플레이어의 인기도
        /// </summary>
        public int Popularity { get; private set; }

        /// <summary>
        /// 무릉 층 수. 만약 무릉을 클리어 한지 너무 오래되었거나 데이터가 없으면 0을 반환한다.
        /// </summary>
        public int DojangFloor { get; private set; }

        /// <summary>
        /// 플레이어의 유니온 레벨. 본캐에서만 유니온 레벨이 보이며, 데이터가 없을 경우 0을 반환한다.
        /// </summary>
        public int UnionLevel { get; private set; }

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
                string lastUpdatedRaw = html.DocumentNode.SelectSingleNode("//div[@class=\"mb-1 text-white\"]/span")?.InnerText;
                int? lastUpdated = lastUpdatedRaw?.GetDigit();

                HtmlNodeCollection profile = html.DocumentNode.SelectNodes("//ul[@class=\"user-summary-list\"]/li");
                string levelRaw = profile[0].InnerText;
                int level = levelRaw.GetDigit();

                string job = profile[1].InnerText.Trim();

                string popularityRaw = profile[2].InnerText;
                int popularity = popularityRaw.GetDigit();

                HtmlNodeCollection datas = html.DocumentNode.SelectNodes("//section[@class=\"box user-summary-box\"]");

                string dojangFloorRaw = datas[0].SelectSingleNode(".//h1")?.InnerText ?? "0";
                int dojangFloor = dojangFloorRaw.GetDigit();

                string unionRaw = datas[2].SelectSingleNode(".//span[@class=\"user-summary-level\"]")?.InnerText ?? "0";
                int union = unionRaw.GetDigit();

                LastUpdated = lastUpdated;
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
