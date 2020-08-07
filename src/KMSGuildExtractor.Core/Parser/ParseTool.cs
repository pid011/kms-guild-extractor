using System.Text.RegularExpressions;

namespace KMSGuildExtractor.Core.Parser
{
    internal static class ParseTool
    {
        public static int GetDigitInString(string str) =>
                int.TryParse(Regex.Replace(str, @"[^0-9]", string.Empty), out int result) ? result : 0;
    }
}
