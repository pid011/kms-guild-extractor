using System;

namespace KMSGuildExtractor.Core
{
    [Serializable]
    public class UserSyncException : Exception
    {
        public string Name { get; }

        public UserSyncException(string name, string message) : base(message)
        {
            Name = name;
        }
    }
}
