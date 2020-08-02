namespace KMSGuildExtractor.Core.Info
{
    public class GuildUser : User
    {
        public GuildPosition Position { get; }

        public GuildUser(GuildPosition position, string name)
            : this(position, name, 0, string.Empty) { }

        public GuildUser(GuildPosition position, string name, int level, string job)
            : this(position, name, level, job, 0, 0) { }

        public GuildUser(GuildPosition position, string name, int level, string job, int overallRank, int worldRank)
            : base(name, level, job, overallRank, worldRank) => Position = position;
    }
}
