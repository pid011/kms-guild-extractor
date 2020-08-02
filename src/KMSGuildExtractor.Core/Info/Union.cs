namespace KMSGuildExtractor.Core.Info
{
    public class Union : IRank
    {
        public int Level { get; }
        public int OverallRank { get; }
        public int WorldRank { get; }

        public Union(int level) : this(level, 0, 0) { }

        public Union(int level, int overallRank, int worldRank)
        {
            Level = level;
            OverallRank = overallRank;
            WorldRank = worldRank;
        }
    }
}
