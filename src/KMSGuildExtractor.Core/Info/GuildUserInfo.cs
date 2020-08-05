namespace KMSGuildExtractor.Core.Info
{
    public class GuildUserInfo : UserInfo
    {
        public GuildPosition Position { get; set; }

        public GuildUserInfo(string name, WorldID world) : base(name, world) { }
    }
}
