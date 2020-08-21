using System.IO;
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
            using FileStream fs = new FileStream(path, FileMode.Create, FileAccess.Write);
            using StreamWriter writer = new StreamWriter(fs, new UTF8Encoding(encoderShouldEmitUTF8Identifier: true)); // utf-8-bom

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

            foreach ((GuildPosition position, User user) in guildData.Members)
            {
                builder = builder
                    .AppendJoin(Seperater,
                                GetCSVString(user.Name),
                                GetCSVString(position.ToLocalizedString()),
                                user.Level ?? 0,
                                GetCSVString(user.Job),
                                user.LastUpdated ?? -1,
                                user.Popularity ?? 0,
                                user.DojangFloor ?? 0,
                                user.UnionLevel ?? 0
                                )
                    .AppendLine();
            }

            await writer.WriteAsync(builder);
        }

        private static string GetCSVString<T>(T obj) => obj is null ? "\"---\"" : $"\"{obj}\"";
    }
}
