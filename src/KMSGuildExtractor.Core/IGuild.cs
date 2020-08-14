using System.Collections.Generic;

namespace KMSGuildExtractor.Core
{
    public interface IGuild : IStatus
    {
        public int GuildID { get; }
        public IList<(GuildPosition position, User data)> Members { get; }
    }
}
