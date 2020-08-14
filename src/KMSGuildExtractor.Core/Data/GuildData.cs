using System.Collections.Generic;

namespace KMSGuildExtractor.Core.Data
{
    public class GuildData : IStatus // TODO: interface로 변경하기
    {
        public string Name { get; }
        public WorldID World { get; }
        public int GuildID { get; }
        public int? Level { get; set; }
        public IList<GuildMemberData> Members { get; protected set; }

        public GuildData(string name, WorldID world, int guildID)
        {
            Name = name;
            World = world;
            GuildID = guildID;
        }
    }
}
