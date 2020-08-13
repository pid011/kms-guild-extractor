using System.Collections.Generic;

namespace KMSGuildExtractor.Core.Data
{
    public class GuildData : IStatus
    {
        public string Name { get; }
        public WorldID World { get; }
        public int GuildID { get; }
        public int? Level { get; set; }
        public IList<GuildUserData> Users { get; }

        public GuildData(string name, WorldID world, int guildID)
        {
            Name = name;
            World = world;
            GuildID = guildID;
            Users = new List<GuildUserData>();
        }
    }
}
