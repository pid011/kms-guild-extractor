namespace KMSGuildExtractor.Core.Info
{
    public class UserInfo : IStatus, IRank
    {
        public string Name { get; }
        public WorldID World { get; }
        public int Level { get; set; }
        public string Job { get; set; }
        public int OverallRank { get; set; }
        public int WorldRank { get; set; }
        public DojangInfo Dojang { get; set; }
        public UnionInfo Union { get; set; }
        public AchievementInfo Achievement { get; set; }

        public UserInfo(string name, WorldID world)
        {
            Name = name;
            World = world;
        }
    }
}
