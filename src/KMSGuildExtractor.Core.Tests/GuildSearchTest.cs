
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GuildSearchTest : WebParsingTest
    {
        [TestMethod]
        public void GuildSearchTest1()
        {
            Guild guild = Guild.SearchAsync("고잉메리호", WorldID.Reboot).Result;

            Assert.AreEqual(guild.GuildID, 2210);
        }

        //[TestMethod]
        //public void GuildSearchTest2()
        //{
        //    try
        //    {
        //        Guild guild = Guild.SearchAsync("고잉메", WorldID.Reboot).Result;
        //    }
        //    catch (AggregateException e) when (e.InnerException is ParseException pe)
        //    {
        //        Assert.AreEqual(pe.Message, "Failed to parse guild search html");
        //    }
        //}

        //[TestMethod]
        //public void GuildSearchTest3()
        //{
        //    try
        //    {
        //        Guild guild = Guild.SearchAsync("고잉메리호", WorldID.Scania).Result;
        //    }
        //    catch (AggregateException e) when (e.InnerException is ParseException pe)
        //    {
        //        Assert.AreEqual(pe.Message, "Failed to parse guild search html");
        //    }
        //}

        //[TestMethod]
        //public void GuildSearchTest4()
        //{
        //    Guild guild = Guild.SearchAsync("훈장교", WorldID.Scania).Result;

        //    Assert.AreEqual(guild.GuildID, 241077);
        //}
    }
}
