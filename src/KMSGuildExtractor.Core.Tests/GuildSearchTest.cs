using System.Threading;
using System.Threading.Tasks;

using KMSGuildExtractor.Core.Info;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildSearchTest
    {

        [TestMethod]
        public void GuildSearchTest1()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.SearchGuildAsync("고잉메리호", WorldID.Reboot, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result.GuildID, 2210);
        }

        [TestMethod]
        public void GuildSearchTest2()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.SearchGuildAsync("고잉메", WorldID.Reboot, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result, null);
        }

        [TestMethod]
        public void GuildSearchTest3()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.SearchGuildAsync("훈장교", WorldID.Scania, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result.GuildID, 241077);
        }

        [TestMethod]
        public void GuildSearchTest4()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.SearchGuildAsync("사과", WorldID.Burning, tokenSource.Token);
            Task.WaitAll(task);

            Assert.AreEqual(task.Result.GuildID, 2);
        }

    }
}