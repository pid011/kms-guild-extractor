using System.Collections.Generic;

namespace KMSGuildExtractor.Core
{
    public interface IGuild : IStatus
    {
        public int GuildID { get; }
        public IReadOnlyList<GuildMember> Members { get; }
    }
}
