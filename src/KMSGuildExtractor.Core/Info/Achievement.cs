namespace KMSGuildExtractor.Core.Info
{
    public class Achievement : IRank
    {
        public int Score { get; }
        public int OverallRank { get; }
        public int WorldRank { get; }

        public Achievement(int score) : this(score, 0, 0) { }

        public Achievement(int score, int overallRank, int worldRank)
        {
            Score = score;
            OverallRank = overallRank;
            WorldRank = worldRank;
        }
    }
}
