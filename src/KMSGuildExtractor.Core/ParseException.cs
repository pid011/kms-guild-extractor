using System;

namespace KMSGuildExtractor.Core
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException(string message, Exception inner) : base(message, inner) { }
    }
}
