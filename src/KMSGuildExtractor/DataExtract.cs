using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor
{
    public class DataExtract
    {
        private const char Seperater = ',';

        public static async Task CreateCSVAsync(string path, IGuild guildData)
        {
            using FileStream fs = new(path, FileMode.Create, FileAccess.Write);
            using StreamWriter writer = new(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); // utf-8-bom

            StringBuilder builder = new StringBuilder()
                .AppendJoin(Seperater,
                            LocalizationString.data_user_name,
                            LocalizationString.data_position,
                            LocalizationString.data_level,
                            LocalizationString.data_job,
                            LocalizationString.data_lastupdated,
                            LocalizationString.data_popularity,
                            LocalizationString.data_dojang_floor,
                            LocalizationString.data_union_level,
                            "maple.gg")
                .AppendLine();

            foreach (GuildMember member in guildData.Members.OrderBy(m => (int)m.Position))
            {
                builder
                    .AppendJoin(
                        Seperater,
                        GetCSVString(member.Info.Name),
                        GetCSVString(member.Position.ToLocalizedString()),
                        GetCSVString(member.Info.Level ?? 0),
                        GetCSVString(member.Info.Job),
                        GetCSVString(member.Info.LastUpdated ?? -1),
                        GetCSVString(member.Info.Popularity ?? 0),
                        GetCSVString(member.Info.DojangFloor ?? 0),
                        GetCSVString(member.Info.UnionLevel ?? 0),
                        GetCSVString($"https://maple.gg/u/{member.Info.Name}"))
                    .AppendLine();
            }

            await writer.WriteAsync(builder);
        }

        private static string GetCSVString<T>(T obj)
        {
            if (obj is null)
            {
                return "NULL";
            }

            return obj is string ? $"\"{obj}\"" : $"{obj}";
        }
    }
}
