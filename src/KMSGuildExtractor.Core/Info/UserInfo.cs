using System;

namespace KMSGuildExtractor.Core.Info
{
    public class UserInfo : IStatus
    {
        public DateTime LastUpdated { get; set; }
        public WorldID World { get; }
        public string Name { get; }
        public string Job { get; set; }
        public int? Level { get; set; }
        public int? Popularity { get; set; }
        public int? DojangFloor { get; set; }
        public int? UnionLevel { get; set; }

        public UserInfo(string name, WorldID world)
        {
            Name = name;
            World = world;
        }
    }
}
