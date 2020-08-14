using System;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildSearchTest : WebParsingTest
    {
        [TestMethod]
        public void GuildSearchTest1()
        {
            Task<Guild> searchTask = Guild.SearchAsync("고잉메리호", WorldID.Reboot);
            Guild guild = searchTask.Result;

            Assert.AreEqual(guild.GuildID, 2210);
        }

        [TestMethod]
        public void GuildSearchTest2()
        {
            try
            {
                Task<Guild> searchTask = Guild.SearchAsync("고잉메", WorldID.Reboot);
                Guild guild = searchTask.Result;
            }
            catch (AggregateException e) when (e.InnerException is ParseException pe)
            {
                Assert.AreEqual(pe.Message, "Failed to parse guild search html");
            }
        }

        [TestMethod]
        public void GuildSearchTest3()
        {
            try
            {
                Task<Guild> searchTask = Guild.SearchAsync("고잉메리호", WorldID.Scania);
                Guild guild = searchTask.Result;
            }
            catch (AggregateException e) when (e.InnerException is ParseException pe)
            {
                Assert.AreEqual(pe.Message, "Failed to parse guild search html");
            }
        }

        [TestMethod]
        public void GuildSearchTest4()
        {
            Task<Guild> searchTask = Guild.SearchAsync("훈장교", WorldID.Scania);
            Guild guild = searchTask.Result;

            Assert.AreEqual(guild.GuildID, 241077);
        }
    }
}
