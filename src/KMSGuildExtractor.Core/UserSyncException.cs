using System;

namespace KMSGuildExtractor.Core
{

    [Serializable]
    public class UserSyncException : Exception
    {
        public UserSyncException(string message) : base(message) { }
    }
}
