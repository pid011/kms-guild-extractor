using System.Collections.Generic;

namespace KMSGuildExtractor.Core.Info
{
    public class Guild
    {
        public string Name { get; }
        public WorldID World { get; }
        public int Level { get; }
        public IList<GuildUser> Users { get; set; }

        public Guild(string name, WorldID world) : this(name, world, 0) { } 

        public Guild(string name, WorldID world, int level)
        {
            Name = name;
            World = world;
            Level = level;
        }
    }
}
