using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class UserDataParsingTest : WebParsingTest
    {
        [TestMethod]
        public void UserDataParsingTest1()
        {
            User user = new("캡틴이름뭐해", WorldID.Reboot);
            user.LoadUserDetailAsync().Wait();

            Assert.IsNotNull(user.LastUpdated);
            Assert.IsNotNull(user.Level);
            Assert.IsNotNull(user.Job);
            Assert.IsNotNull(user.Popularity);
            Assert.IsNotNull(user.DojangFloor);
            Assert.IsNotNull(user.UnionLevel);
        }
    }
}
