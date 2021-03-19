using System;
using System.Threading;
using System.Threading.Tasks;

using KMSGuildExtractor.Core.Requester;

namespace KMSGuildExtractor.Core.Utils
{
    public static class Update
    {
        public static async Task<(bool compare, string url)> CompareVersionAsync(Version target,
                                                                                 CancellationToken cancellationToken = default)
        {
            if (target is null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            string versionString = $"v{target.ToString(3)}";
            GitHubRequester.ReleaseData data = await GitHubRequester.GetLastReleaseAsync(cancellationToken);

            if (data is null)
            {
                return (false, null);
            }

            bool compare = data != null && versionString.CompareTo(data.TagName) >= 0;
            string url = data?.Url;

            return (compare, url);
        }
    }
}
