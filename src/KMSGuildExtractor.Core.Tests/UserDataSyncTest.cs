using System.Threading;
using System.Threading.Tasks;

using KMSGuildExtractor.Core.Requester;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class UserDataSyncTest : WebParsingTest
    {
        [TestMethod]
        public void UserDataSyncNotFound1()
        {
            UserDataRequester.SyncData check = WaitForSyncing("일이삼사오육칠");

            Assert.AreEqual(check.Error, true);
            Assert.AreEqual(check.Message, "캐릭터를 찾을 수 없습니다.");
        }

        [TestMethod]
        public void UserDataSyncNotFound2()
        {
            UserDataRequester.SyncData check = WaitForSyncing("캡틴이름뮈헤");

            Assert.AreEqual(check.Error, true);
            Assert.AreEqual(check.Message, "갱신에 실패했습니다. (캐릭터를 찾을 수 없습니다.)");
        }

        [TestMethod]
        public void UserDataSyncNotFound3()
        {
            UserDataRequester.SyncData check = WaitForSyncing("캡틴이름뭐해");

            Assert.AreEqual(check.Done, true);
        }

        private UserDataRequester.SyncData WaitForSyncing(string name)
        {
            while (true)
            {
                Task<UserDataRequester.SyncData> data = UserDataRequester.GetUserSyncDataAsync(name, new CancellationTokenSource().Token);
                Task.WaitAll(data);
                if (data.Result.Error == true || data.Result.Done == true)
                {
                    return data.Result;
                }
                Task.WaitAll(Task.Delay(data.Result.Interval));
            }
        }
    }
}
