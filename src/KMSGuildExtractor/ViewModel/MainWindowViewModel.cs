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

        public ObservableCollection<World> WorldList { get; } = new ObservableCollection<World>
        {
            new World(LocalizationString.world_luna, WorldID.Luna),

            new World(LocalizationString.world_scania, WorldID.Scania),

            new World(LocalizationString.world_elysium, WorldID.Elysium),

            new World(LocalizationString.world_croa, WorldID.Croa),

            new World(LocalizationString.world_aurora, WorldID.Aurora),

            new World(LocalizationString.world_bera, WorldID.Bera),

            new World(LocalizationString.world_red, WorldID.Red),

            new World(LocalizationString.world_union, WorldID.Union),

            new World(LocalizationString.world_zenith, WorldID.Zenith),

            new World(LocalizationString.world_enosis, WorldID.Enosis),

            new World(LocalizationString.world_nova, WorldID.Nova),

            new World(LocalizationString.world_reboot, WorldID.Reboot),

            new World(LocalizationString.world_reboot2, WorldID.Reboot2),

            new World(LocalizationString.world_burning, WorldID.Burning),

            new World(LocalizationString.world_burning2, WorldID.Burning2)
        };

        public World SelectedWorld
        {
            get => _selectedWorld;
            set => SetProperty(ref _selectedWorld, value, nameof(SelectedWorld));
        }

        private World _selectedWorld;

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

        public class World
        {
            public string Name { get; }

            public string WorldLogoPath =>
                $"pack://application:,,,/resources/icons/worlds/{Url.ToString().ToLower()}.png";

            public WorldID Url { get; }

            public World(string name, WorldID url)
            {
                Name = name;
                Url = url;
            }
        }
    }
}
