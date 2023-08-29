using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class UserDataParsingTest : WebParsingTest
    {
        [TestMethod]
        public void UserDataParsingTest1()
        {
            User user = new("진캐", WorldID.Scania);
            user.LoadUserDetailAsync().Wait();

            Assert.IsNotNull(user.LastUpdated);
            Assert.IsNotNull(user.Level);
            Assert.IsNotNull(user.Job);
            Assert.IsNotNull(user.Popularity);
            Assert.IsNotNull(user.DojangFloor);
            Assert.IsNotNull(user.UnionLevel);
        }

        [TestMethod]
        public void LevelDataParsingTest()
        {
            var levelRaw = "Lv.260(37.014%)";
            Assert.IsNotNull(levelRaw.ParseLevel());
        }
    }
}
