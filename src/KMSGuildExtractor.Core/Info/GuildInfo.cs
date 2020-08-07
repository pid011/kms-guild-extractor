using System.Collections.Generic;

namespace KMSGuildExtractor.Core.Info
{
    public class GuildInfo : IStatus
    {
        public string Name { get; }
        public WorldID World { get; }
        public int GuildID { get; }
        public int? Level { get; set; }
        public IList<GuildUserInfo> Users { get; }

        public GuildInfo(string name, WorldID world, int guildID)
        {
            Name = name;
            World = world;
            GuildID = guildID;
            Users = new List<GuildUserInfo>();
        }
    }
}
