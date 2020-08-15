using System;
using System.Threading.Tasks;
using System.Windows;

using KMSGuildExtractor.Core.Utils;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public string Title => $"{LocalizationString.title} {VersionString}";

        public Version AppVersion => typeof(App).Assembly.GetName().Version;

        public string VersionString => $"v{AppVersion.ToString(3)}";

        public string UpdateStatus
        {
            get => _updateStatus;
            private set => SetProperty(ref _updateStatus, value ?? string.Empty, nameof(UpdateStatus));
        }

        private string _updateStatus = LocalizationString.updatenotify_check_update;

        public Visibility ReleaseLinkVisible
        {
            get => _releaseLinkVisible;
            private set => SetProperty(ref _releaseLinkVisible, value, nameof(ReleaseLinkVisible));
        }

        private Visibility _releaseLinkVisible = Visibility.Collapsed;

        public Uri ReleaseLink
        {
            get => _releaseLink;
            private set => SetProperty(ref _releaseLink, value, nameof(ReleaseLink));
        }

        private Uri _releaseLink;

        public MainWindowViewModel()
        {
            Task.Run(InitializeUpdateStatus);
        }

        private async Task InitializeUpdateStatus()
        {
            (bool compare, string url) = await Update.CompareVersionAsync(AppVersion);

            if (compare)
            {
                UpdateStatus = LocalizationString.updatenotify_already_updated;
                ReleaseLinkVisible = Visibility.Collapsed;
                return;
            }

            ReleaseLink = new Uri(url);
            UpdateStatus = LocalizationString.updatenotify_new_version_update;
            ReleaseLinkVisible = Visibility.Visible;
        }
    }
}
