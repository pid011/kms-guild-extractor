using System.Threading;
using System.Threading.Tasks;

using HtmlAgilityPack;

using KMSGuildExtractor.Core.Info;
using KMSGuildExtractor.Core.Parser;
using KMSGuildExtractor.Core.Requester;

namespace KMSGuildExtractor.Core
{
    public static class User
    {
        public static async Task SyncUserDataAsync(string name, CancellationToken cancellation)
        {
            while (!cancellation.IsCancellationRequested)
            {
                UserDataRequester.SyncData data = await UserDataRequester.GetUserSyncDataAsync(name, cancellation)
                    ?? new UserDataRequester.SyncData { Error = true };

                if (data.Error)
                {
                    // Exception 새로 만들어서 메시지 리턴하기
                    return;
                }
                if (data.Done)
                {
                    return;
                }
                await Task.Delay(data.Interval, cancellation);
            }
        }

        public static async Task<UserInfo> GetUserDataAsync(string name, WorldID world, CancellationToken cancellation)
        {
            UserInfo user = new UserInfo(name, world);
            HtmlDocument html = await UserDataRequester.GetUserDataHtmlAsync(name, cancellation);
            return UserDataParser.TryGetUserDetail(ref user, html) ? user : null;
        }
    }
}
