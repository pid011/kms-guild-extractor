using System.IO;
using System.Reflection;

namespace KMSGuildExtractor.Utils
{
    internal class ProgramPath
    {
        internal static string ProgramDirectoryPath =>
            Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
    }
}
