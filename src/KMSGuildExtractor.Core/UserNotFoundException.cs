using System;

namespace KMSGuildExtractor.Core
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public string Name { get; }

        public UserNotFoundException(string name) => Name = name;
    }
}
