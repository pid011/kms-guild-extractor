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
            return (versionString == data?.TagName, data?.Url);
        }
    }
}
