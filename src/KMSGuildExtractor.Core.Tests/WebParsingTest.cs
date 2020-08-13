using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KMSGuildExtractor.Core.Tests
{
    [TestClass]
    public class WebParsingTest
    {
        [TestInitialize]
        public void WaitBeforeTest()
        {
            Task.WaitAll(Task.Delay(1500));
        }
    }
}
