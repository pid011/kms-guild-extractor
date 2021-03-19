using System;

namespace KMSGuildExtractor.Core
{
    [Serializable]
    public class UserSyncException : Exception
    {
        public string Name { get; }
        public string RawJsonValue { get; }

        public UserSyncException(string name, string message) : base(message)
        {
            Name = name;
            RawJsonValue = null;
        }

        public UserSyncException(string name, string message, Exception innerException) : base(message, innerException)
        {
            Name = name;
            RawJsonValue = null;
        }

        public UserSyncException(string name, string message, Exception innerException, string rawJson) : base(message, innerException)
        {
            Name = name;
            RawJsonValue = rawJson;
        }
    }
}
