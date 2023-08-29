using System.Text.RegularExpressions;

namespace KMSGuildExtractor.Core
{
    internal static partial class StringExtensions
    {
        /// <summary>
        /// maple.gg에서 가져온 문자열에서 레벨을 파싱하여 반환합니다.
        /// </summary>
        /// <param name="str">파싱할 문자열</param>
        /// <returns>파싱된 문자열</returns>
        public static int? ParseLevel(this string str)
        {
            Match captured = LevelParser().Match(str);
            if (!captured.Success || !int.TryParse(captured.Value[3..], out int parsed))
            {
                return null;
            }

            return parsed;
        }

        public static int? ParseInt(this string str)
        {
            return int.TryParse(IntegerParser().Replace(str, string.Empty), out int result) ? result : null;
        }

        [GeneratedRegex("Lv\\.[0-9]{3}")]
        private static partial Regex LevelParser();
        [GeneratedRegex("[^0-9\\-]")]
        private static partial Regex IntegerParser();
    }
}
