using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using KMSGuildExtractor.Core.Data;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildDetailParsingTest : WebParsingTest
    {
        [TestMethod]
        public void GuildDetailParsingTest1()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildData> task = Guild.GetGuildDetailAsync(new GuildData("고잉메리호", WorldID.Reboot, 2210), tokenSource.Token);
            Task.WaitAll(task);

            Assert.IsTrue(task.Result.Users.Any(u => u.Name == "캡틴이름뭐해"));
            //Assert.AreEqual(task.Result.Users.Count, 171);
        }

        [TestMethod]
        public void GuildDetailParsingTest2()
        {
            var tokenSource = new CancellationTokenSource();
            Task<GuildData> task = Guild.GetGuildDetailAsync(new GuildData("훈장교", WorldID.Scania, 241077), tokenSource.Token);
            Task.WaitAll(task);

            Assert.IsTrue(task.Result.Users.Any(u => u.Name == "신남" && u.Position == GuildPosition.Owner));
            //Assert.AreEqual(task.Result.Users.Count, 174);
        }
    }
}
