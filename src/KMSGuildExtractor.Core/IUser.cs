namespace KMSGuildExtractor.Core
{
    public interface IUser : IStatus
    {
        public int? LastUpdated { get; }
        public string? Job { get; }
        public int? Popularity { get; }
        public int? DojangFloor { get; }
        public int? UnionLevel { get; }
    }
}
