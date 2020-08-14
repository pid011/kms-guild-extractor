using System.Linq;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildDetailParsingTest : WebParsingTest
    {
        [TestMethod]
        public void GuildDetailParsingTest1()
        {
            Task<Guild> searchTask = Guild.SearchAsync("고잉메리호", WorldID.Reboot);
            Guild guild = searchTask.Result;

            Task loadTask = guild.LoadGuildMembersAsync();
            Task.WaitAll(loadTask);

            Assert.IsTrue(guild.Members.Any(u => u.data.Name == "캡틴이름뭐해"));
        }

        [TestMethod]
        public void GuildDetailParsingTest2()
        {
            Task<Guild> searchTask = Guild.SearchAsync("훈장교", WorldID.Scania);
            Guild guild = searchTask.Result;

            Task loadTask = guild.LoadGuildMembersAsync();
            Task.WaitAll(loadTask);

            Assert.IsTrue(guild.Members.Any(u => u.data.Name == "신남" && u.position == GuildPosition.Owner));
        }
    }
}
