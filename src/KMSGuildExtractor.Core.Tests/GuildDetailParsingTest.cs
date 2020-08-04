using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KMSGuildExtractor.Core;
using KMSGuildExtractor.Core.Info;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildDetailParsingTest
    {

        [TestMethod]
        public void GuildDetailParsingTest1()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.GetGuildDetailAsync(new GuildInfo("°íÀ×¸Þ¸®È£", WorldID.Reboot, 2210), tokenSource.Token);
            Task.WaitAll(task);

            //Assert.IsTrue(task.Result.Users.Any(u => u.Name == "Ä¸Æ¾ÀÌ¸§¹¹ÇØ"));
            Assert.AreEqual(task.Result.Users.Count, 171);
        }

        [TestMethod]
        public void GuildDetailParsingTest2()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildInfo> task = Guild.GetGuildDetailAsync(new GuildInfo("ÈÆÀå±³", WorldID.Scania, 241077), tokenSource.Token);
            Task.WaitAll(task);

            //Assert.IsTrue(task.Result.Users.Any(u => u.Name == "½Å³²" && u.Position == GuildPosition.Master));
            Assert.AreEqual(task.Result.Users.Count, 174);
        }
    }
}
