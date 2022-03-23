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
                            GetCSVString(LocalizationString.data_user_name),
                            GetCSVString(LocalizationString.data_position),
                            GetCSVString(LocalizationString.data_level),
                            GetCSVString(LocalizationString.data_job),
                            GetCSVString(LocalizationString.data_lastupdated),
                            GetCSVString(LocalizationString.data_popularity),
                            GetCSVString(LocalizationString.data_dojang_floor),
                            GetCSVString(LocalizationString.data_union_level)
                            )
                .AppendLine();

            foreach (GuildMember member in guildData.Members.OrderBy(m => (int)m.Position))
            {
                builder
                    .AppendJoin(
                        Seperater,
                        GetCSVString(member.Info.Name),
                        GetCSVString(member.Position.ToLocalizedString()),
                        member.Info.Level ?? 0,
                        GetCSVString(member.Info.Job),
                        member.Info.LastUpdated ?? -1,
                        member.Info.Popularity ?? 0,
                        member.Info.DojangFloor ?? 0,
                        member.Info.UnionLevel ?? 0)
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

            return obj.ToString().IndexOf(Seperater) < 0
                ? $"{obj}"
                : $"\"{obj}\"";
        }
    }
}
