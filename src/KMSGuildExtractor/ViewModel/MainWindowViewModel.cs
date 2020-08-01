using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public string Title => $"{LocalizationString.title} {Version}";

        public string Version { get; } = $"v{typeof(App).Assembly.GetName().Version.ToString(3)}";

        public string UpdateStatus
        {
            get => _updateStatus;
            private set => SetProperty(ref _updateStatus, value, nameof(UpdateStatus));
        }
        private string _updateStatus;

        public ObservableCollection<Server> ServerList { get; } = new ObservableCollection<Server>
        {
            new Server(LocalizationString.server_luna, ServerID.Luna),

            new Server(LocalizationString.server_scania, ServerID.Scania),

            new Server(LocalizationString.server_elysium, ServerID.Elysium),

            new Server(LocalizationString.server_croa, ServerID.Croa),

            new Server(LocalizationString.server_aurora, ServerID.Aurora),

            new Server(LocalizationString.server_bera, ServerID.Bera),

            new Server(LocalizationString.server_red, ServerID.Red),

            new Server(LocalizationString.server_union, ServerID.Union),

            new Server(LocalizationString.server_zenith, ServerID.Zenith),

            new Server(LocalizationString.server_enosis, ServerID.Enosis),

            new Server(LocalizationString.server_nova, ServerID.Nova),

            new Server(LocalizationString.server_reboot, ServerID.Reboot),

            new Server(LocalizationString.server_reboot2, ServerID.Reboot2),

            new Server(LocalizationString.server_burning, ServerID.Burning),

            new Server(LocalizationString.server_burning2, ServerID.Burning2)
        };

        public Server SelectedServer
        {
            get => _selectedServer;
            set => SetProperty(ref _selectedServer, value, nameof(SelectedServer));
        }

        private Server _selectedServer;

        public string GuildName
        {
            get => _guildName;
            set => SetProperty(ref _guildName, value, nameof(GuildName));
        }

        private string _guildName = string.Empty;

        public string GuildNameCheck
        {
            get => _guildNameCheck;
            set => SetProperty(ref _guildNameCheck, value, nameof(GuildNameCheck));
        }

        private string _guildNameCheck = string.Empty;

        public ICommand SearchCommand { get; } = new Command(ExecuteSearchCommand, CanExecuteSearchCommand);

        public MainWindowViewModel()
        {
            UpdateStatus = LocalizationString.updatenotify_check_update;
            PropertyChanged += MainWindowViewModel_PropertyChanged;
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GuildName))
            {
                GuildNameCheck = IsValidGuildName(GuildName) ? string.Empty : LocalizationString.input_wrong_guild_name;

                if (GuildNameCheck == string.Empty)
                {

                }
            }
        }

        private bool IsValidGuildName(string guildName)
        {
            if (!Regex.IsMatch(guildName, "^[0-9a-zA-Z가-힣]*$")) // 특수문자 입력 금지
            {
                return false;
            }

            const float limit = 6;

            return guildName.ToCharArray()
                            .Sum(ch => Regex.IsMatch(ch.ToString(), "[0-9a-zA-Z]") ? 0.5f : 1f)
                            <= limit;
        }

        private static bool CanExecuteSearchCommand(object arg)
        {
            return true;
        }

        private static void ExecuteSearchCommand(object obj)
        {
            MessageBox.Show("Command ExecuteMethod");
        }

        public class Server
        {
            public string Name { get; }

            public string ServerLogoPath =>
                $"pack://application:,,,/resources/icons/worlds/{Url.ToString().ToLower()}.png";

            public ServerID Url { get; }

            public Server(string name, ServerID url)
            {
                Name = name;
                Url = url;
            }
        }
    }
}
