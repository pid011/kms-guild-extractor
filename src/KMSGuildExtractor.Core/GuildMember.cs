namespace KMSGuildExtractor.Core
{
    public record GuildMember
    {
        public GuildPosition Position { get; }
        public User Info { get; }

        public GuildMember(GuildPosition position, User info)
        {
            Position = position;
            Info = info;
        }
    }
}
