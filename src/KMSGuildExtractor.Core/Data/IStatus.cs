namespace KMSGuildExtractor.Core.Data
{
    public interface IStatus
    {
        public WorldID World { get; }
        public string Name { get; }
        public int? Level { get; set; }
    }
}
