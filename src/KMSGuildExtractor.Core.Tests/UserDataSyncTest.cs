using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class UserDataSyncTest : WebParsingTest
    {
        // Sync 기능이 작동하지 않아 임시로 테스트 제거
        //[TestMethod]
        //public void UserDataSyncTest1()
        //{
        //    UserDataRequester.SyncData check = WaitForSyncing("일이삼사오육칠");

        //    Assert.AreEqual(check.Error, true);
        //    Assert.AreEqual(check.Message, "캐릭터를 찾을 수 없습니다.");
        //}

        //[TestMethod]
        //public void UserDataSyncTest2()
        //{
        //    UserDataRequester.SyncData check = WaitForSyncing("캡틴이름뮈헤");

        //    Assert.AreEqual(check.Error, true);
        //    Assert.AreEqual(check.Message, "갱신에 실패했습니다. (캐릭터를 찾을 수 없습니다.)");
        //}

        //[TestMethod]
        //public void UserDataSyncTest3()
        //{
        //    UserDataRequester.SyncData check = WaitForSyncing("캡틴이름뭐해");

        //    Assert.AreEqual(check.Done, true);
        //}

        //private static UserDataRequester.SyncData WaitForSyncing(string name)
        //{
        //    while (true)
        //    {
        //        UserDataRequester.SyncData data = UserDataRequester.GetUserSyncDataAsync(name, new CancellationTokenSource().Token).Result;
        //        if (data.Error == true || data.Done == true)
        //        {
        //            return data;
        //        }
        //        Task.Delay(data.Interval ?? 2000, new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token).Wait();
        //    }
        //}
    }
}
