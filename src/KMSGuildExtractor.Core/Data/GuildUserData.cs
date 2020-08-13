namespace KMSGuildExtractor.Core.Data
{
    public class GuildUserData : UserData
    {
        public GuildPosition Position { get; set; }

        public GuildUserData(string name, WorldID world) : base(name, world) { }
    }
}
