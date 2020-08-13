using System.Threading;
using System.Threading.Tasks;

using KMSGuildExtractor.Core.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class UserDataParsingTest : WebParsingTest
    {
        [TestMethod]
        public void UserDataParsingTest1()
        {
            var tokenSource = new CancellationTokenSource();
            Task<UserData> task = User.GetUserDataAsync("캡틴이름뭐해", WorldID.Reboot, tokenSource.Token);
            Task.WaitAll(task);

            Assert.IsNotNull(task.Result.LastUpdated);
            Assert.IsNotNull(task.Result.Level);
            Assert.IsNotNull(task.Result.Job);
            Assert.IsNotNull(task.Result.Popularity);
            Assert.IsNotNull(task.Result.DojangFloor);
            Assert.IsNotNull(task.Result.UnionLevel);
        }

        [TestMethod]
        public void UserDataParsingTest2()
        {
            var tokenSource = new CancellationTokenSource();
            Task<UserData> task = User.GetUserDataAsync("킹갓일", WorldID.Reboot, tokenSource.Token);
            Task.WaitAll(task);

            Assert.IsNotNull(task.Result.LastUpdated);
            Assert.IsNotNull(task.Result.Level);
            Assert.IsNotNull(task.Result.Job);
            Assert.IsNotNull(task.Result.Popularity);
            Assert.IsNotNull(task.Result.DojangFloor);
            Assert.IsNotNull(task.Result.UnionLevel);
        }

        [TestMethod]
        public void UserDataParsingTest3()
        {
            var tokenSource = new CancellationTokenSource();
            Task<UserData> task = User.GetUserDataAsync("클라스", WorldID.Scania, tokenSource.Token);
            Task.WaitAll(task);

            Assert.IsNotNull(task.Result.LastUpdated);
            Assert.IsNotNull(task.Result.Level);
            Assert.IsNotNull(task.Result.Job);
            Assert.IsNotNull(task.Result.Popularity);
            Assert.IsNotNull(task.Result.DojangFloor);
            Assert.IsNotNull(task.Result.UnionLevel);
        }
    }
}
