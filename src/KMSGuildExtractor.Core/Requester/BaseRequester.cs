using System.Net.Http;

using HtmlAgilityPack;

namespace KMSGuildExtractor.Core.Requester
{
    internal class BaseRequester
    {
        protected static readonly HtmlWeb s_web = new();
        protected static readonly HttpClient s_client = new();
    }
}
