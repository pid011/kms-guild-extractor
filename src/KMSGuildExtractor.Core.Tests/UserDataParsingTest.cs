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
            User user = new User("캡틴이름뭐해", WorldID.Reboot);
            Task loadTask = user.LoadUserDetailAsync();
            Task.WaitAll(loadTask);

            Assert.IsNotNull(user.LastUpdated);
            Assert.IsNotNull(user.Level);
            Assert.IsNotNull(user.Job);
            Assert.IsNotNull(user.Popularity);
            Assert.IsNotNull(user.DojangFloor);
            Assert.IsNotNull(user.UnionLevel);
        }
    }
}
