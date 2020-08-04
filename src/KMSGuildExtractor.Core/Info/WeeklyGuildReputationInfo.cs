using System;
using System.Collections.Generic;
using System.Text;

namespace KMSGuildExtractor.Core.Info
{
    public class WeeklyGuildReputationInfo : IRank
    {
        public int Score { get; set; }
        public int OverallRank { get; set; }
        public int WorldRank { get; set; }
    }
}
