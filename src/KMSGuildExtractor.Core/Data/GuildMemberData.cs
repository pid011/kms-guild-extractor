namespace KMSGuildExtractor.Core.Data
{
    public class GuildMemberData : UserData
    {
        public GuildPosition Position { get; set; }

        public GuildMemberData(string name, WorldID world) : base(name, world) { }
    }
}
