using System;

namespace KMSGuildExtractor.Core.Parser
{
    [Serializable]
    public class ParseException : Exception
    {
        public ParseException(string message, Exception inner) : base(message, inner) { }
    }
}
