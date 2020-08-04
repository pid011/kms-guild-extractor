using System.Collections.Generic;

namespace KMSGuildExtractor.Core.Info
{
    public class GuildInfo : IStatus
    {
        public string Name { get; }
        public WorldID World { get; }
        public int Level { get; set; }
        public string WeeklyReputation { get; set; }
        public IList<GuildUserInfo> Users { get; }

        public GuildInfo(string name, WorldID world)
        {
            Name = name;
            World = world;
            Users = new List<GuildUserInfo>();
        }
    }
}
