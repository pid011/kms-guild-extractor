using KMSGuildExtractor.Core.Requester;
using KMSGuildExtractor.Core.Utils;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class GitHubRequesterTest : WebParsingTest
    {
        [TestMethod]
        public void GitHubRequestTest()
        {
            GitHubRequester.ReleaseData data = GitHubRequester.GetLastReleaseAsync().Result;
            Assert.IsNotNull(data.Url);
            Assert.IsNotNull(data.TagName);
        }

        [TestMethod]
        public void UpdateCheckTest()
        {
            bool compare = Update.CompareVersionAsync(new System.Version(0, 1, 1)).Result.compare;

            Assert.IsFalse(compare);
        }
    }
}
