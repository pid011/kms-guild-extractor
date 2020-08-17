using System;
using System.Threading.Tasks;
using System.Windows;

using KMSGuildExtractor.Core.Utils;
using KMSGuildExtractor.Localization;
using KMSGuildExtractor.View;

namespace KMSGuildExtractor.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
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

        public object WorkView
        {
            get => _workView;
            set => SetProperty(ref _workView, value, nameof(WorkView));
        }

        private object _workView;

        public MainWindowViewModel()
        {
            WorkView = new SearchView(this);
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
