namespace KMSGuildExtractor.Core.Info
{
    public class User : IStatus, IRank
    {
        public string Name { get; }
        public int Level { get; }
        public string Job { get; }
        public int OverallRank { get; }
        public int WorldRank { get; }
        public Dojang Dojang { get; set; }
        public Union Union { get; set; }
        public Achievement Achievement { get; set; }

        public User(string name) : this(name, 0, string.Empty) { }

        public User(string name, int level, string job) : this(name, level, job, 0, 0) { }

        public User(string name, int level, string job, int overallRank, int worldRank)
        {
            Name = name;
            Level = level;
            Job = job;
            OverallRank = overallRank;
            WorldRank = worldRank;
        }
    }
}
