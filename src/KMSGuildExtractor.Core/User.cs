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
        public string Job { get; private set; }

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

        /// <summary>
        /// maple.gg에 유저 데이터 동기화를 요청한다.
        /// </summary>
        /// <remarks>
        /// 별다른 에러 메시지가 없으면 성공 응답을 받을 때 까지 일정 간격으로 재 요청함.
        /// </remarks>
        /// <param name="cancellation"></param>
        /// <returns>정상적으로 동기화되면 메서드 종료. 아니면 <see cref="UserSyncException"/> 예외 발생</returns>
        /// <exception cref="NullReferenceException">응답으로 받은 json에서 데이터를 파싱하지 못했을 때 발생</exception>
        /// <exception cref="UserSyncException">동기화 실패 응답을 받았을 때 발생. 관련 내용은 <see cref="UserDataRequester.SyncData.Message"/>에 표시됩니다.</exception>
        public async Task RequestSyncAsync(CancellationToken cancellation = default)
        {
            while (true)
            {
                UserDataRequester.SyncData data = await UserDataRequester.GetUserSyncDataAsync(Name, cancellation);

                if (data.Done is true)
                {
                    return;
                }

                if (data.Error is true)
                {
                    throw new UserSyncException(Name, data.Message ?? string.Empty);
                }
                await Task.Delay(data.Interval ?? 2000, cancellation);
            }
        }


        /// <summary>
        /// maple.gg에서 유저 정보를 가져옵니다.
        /// </summary>
        /// <param name="cancellation"></param>
        /// <returns></returns>
        /// <exception cref="ParseException">웹페이지 파싱에 실패했을 때 발생</exception>
        /// <exception cref="UserNotFoundException">maple.gg에서 유저 검색에 실패했을 때 발생</exception>
        public async Task LoadUserDetailAsync(CancellationToken cancellation = default)
        {
            HtmlDocument html = await UserDataRequester.GetUserDataHtmlAsync(Name, cancellation);
            SetUserDetail(html);
        }

        private void SetUserDetail(HtmlDocument html)
        {
            try
            {
                (_, LastUpdated) = ParseProfile(html);
                (Level, Job, Popularity) = ParseInformation(html);
                (DojangFloor, UnionLevel) = ParseTitle(html);
            }
            catch (NullReferenceException) when (html.GetElementbyId("app").SelectSingleNode(".//img[@alt=\"검색결과 없음\"]") != null)
            {
                throw new UserNotFoundException(Name);
            }
            catch (NullReferenceException e)
            {
                throw new ParseException("Faild to parse user detail html", e);
            }

            static (string imgUrl, int? lastUpdated) ParseProfile(HtmlDocument html)
            {
                HtmlNode profile = html.DocumentNode.SelectSingleNode("//div[@class=\"row row-small character-avatar-row\"]");
                string imgUrl = string.Empty; // TODO: Parse character image url
                int? lastUpdated = profile?.SelectSingleNode("./div[2]//span")?.InnerText?.ParseInt();

                return (imgUrl, lastUpdated);
            }

            static (int? level, string job, int? popularity) ParseInformation(HtmlDocument html)
            {
                HtmlNodeCollection information = html.DocumentNode.SelectNodes("//li[@class=\"user-summary-item\"]");
                int? level = information[1]?.InnerText?.ParseLevel();
                string job = information[2]?.InnerText?.Trim();
                int? popularity = information[3]?.InnerText?.ParseInt();

                return (level, job, popularity);
            }

            static (int? dojang, int? union) ParseTitle(HtmlDocument html)
            {
                HtmlNodeCollection title = html.DocumentNode.SelectNodes("//section[@class=\"box user-summary-box\"]");
                int? dojang = title[0].SelectSingleNode("//*[@class=\"user-summary-floor font-weight-bold\"]")?.InnerText?.ParseInt();
                int? union = title[2].SelectSingleNode("//*[@class=\"user-summary-level\"]")?.InnerText?.ParseInt();

                return (dojang, union);
            }
        }
    }
}
