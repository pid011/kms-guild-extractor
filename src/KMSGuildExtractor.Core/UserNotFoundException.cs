using System;

namespace KMSGuildExtractor.Core
{
    [Serializable]
    public class UserNotFoundException : Exception
    {
        public string Name { get; }

        public UserNotFoundException(string name) : base($"Cannot found User '{name}'")
        {
            Name = name;
        }
    }
}
