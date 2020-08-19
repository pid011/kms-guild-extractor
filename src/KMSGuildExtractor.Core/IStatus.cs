namespace KMSGuildExtractor.Core
{
    public interface IStatus
    {
        public WorldID World { get; }
        public string Name { get; }
        public int Level { get; }
    }
}
