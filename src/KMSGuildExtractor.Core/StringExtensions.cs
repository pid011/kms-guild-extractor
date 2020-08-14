using System.Text.RegularExpressions;

namespace KMSGuildExtractor.Core
{
    internal static class StringExtensions
    {
        public static int GetDigit(this string str) =>
                int.TryParse(Regex.Replace(str, @"[^0-9]", string.Empty), out int result) ? result : 0;
    }
}
