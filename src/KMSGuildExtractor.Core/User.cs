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
        /// 플레이어의 레벨. 데이터가 없으면 null을 반환한다.
        /// </summary>
        public int? Level { get; private set; }

        /// <summary>
        /// 플레이어가 마지막으로 활동한 날짜. 현재 날짜에서 마지막 활동일을 뺀 일 수를 반환한다.<br/>
        /// 만약 가져올 수 없다면 null을 반환한다.
        /// </summary>
        public int? LastUpdated { get; private set; }

        /// <summary>
        /// 플레이어의 직업. 데이터가 없으면 null을 반환한다.
        /// </summary>
        public string? Job { get; private set; }

        /// <summary>
        /// 플레이어의 인기도. 데이터가 없으면 null을 반환한다.
        /// </summary>
        public int? Popularity { get; private set; }

        /// <summary>
        /// 무릉 층 수. 만약 무릉을 클리어 한지 너무 오래되었거나 데이터가 없으면 null을 반환한다.
        /// </summary>
        public int? DojangFloor { get; private set; }

        /// <summary>
        /// 플레이어의 유니온 레벨. 본캐에서만 유니온 레벨이 보이며, 데이터가 없으면 null을 반환한다.
        /// </summary>
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
                UserDataRequester.SyncData? data = await UserDataRequester.GetUserSyncDataAsync(Name, cancellation);

                if (data is null)
                {
                    throw new NullReferenceException($"Cannot parse user sync json data : {Name}");
                }

                if (data.Error == true)
                {
                    throw new UserSyncException(Name, data?.Message ?? string.Empty);
                }
                if (data.Done == true)
                {
                    return;
                }
                await Task.Delay(data?.Interval ?? 2000, cancellation);
            }
        }

        private void SetUserDetail(HtmlDocument html)
        {
            try
            {
                int? lastUpdated =
                    html
                    .DocumentNode
                    .SelectSingleNode("//div[@class=\"mb-1 text-white\"]/span")?
                    .InnerText?
                    .GetDigit();

                HtmlNodeCollection profile = html.DocumentNode.SelectNodes("//ul[@class=\"user-summary-list\"]/li");
                int? level = profile[0]?.InnerText?.GetDigit();

                string? job = profile[1]?.InnerText?.Trim();

                int? popularity = profile[2]?.InnerText?.GetDigit();

                HtmlNodeCollection datas = html.DocumentNode.SelectNodes("//section[@class=\"box user-summary-box\"]");

                int? dojangFloor = datas[0].SelectSingleNode(".//h1")?.InnerText?.GetDigit();

                int? union = datas[2].SelectSingleNode(".//span[@class=\"user-summary-level\"]")?.InnerText?.GetDigit();

                LastUpdated = lastUpdated;
                Level = level;
                Job = job;
                Popularity = popularity;
                DojangFloor = dojangFloor;
                UnionLevel = union;
            }
            catch (NullReferenceException) when (html.GetElementbyId("app").SelectSingleNode(".//img[@alt=\"검색결과 없음\"]") != null)
            {
                throw new UserNotFoundException(Name);
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Faild to parse user detail html", e);
            }
        }
    }
}
