namespace KMSGuildExtractor.Core.Info
{
    public class Dojang : IRank
    {
        public int Floor { get; }
        public int ClearTime { get; }
        public int OverallRank { get; }
        public int WorldRank { get; }

        public Dojang(int floor, int clearTime) : this(floor, clearTime, 0, 0) { }

        public Dojang(int floor, int clearTime, int overallRank, int worldRank)
        {
            Floor = floor;
            ClearTime = clearTime;
            OverallRank = overallRank;
            WorldRank = worldRank;
        }
    }
}
