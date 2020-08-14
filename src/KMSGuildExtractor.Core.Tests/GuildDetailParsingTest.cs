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
            Guild guild = Guild.SearchAsync("고잉메리호", WorldID.Reboot).Result;
            guild.LoadGuildMembersAsync().Wait();

            Assert.IsTrue(guild.Members.Any(u => u.data.Name == "캡틴이름뭐해"));
        }

        [TestMethod]
        public void GuildDetailParsingTest2()
        {
            Guild guild = Guild.SearchAsync("훈장교", WorldID.Scania).Result;
            guild.LoadGuildMembersAsync().Wait();

            Assert.IsTrue(guild.Members.Any(u => u.data.Name == "신남" && u.position == GuildPosition.Owner));
        }
    }
}
