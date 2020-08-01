using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;
using System.Threading.Tasks;

namespace KMSGuildExtractor.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public string Title => $"{LocalizationString.title} {Version}";

        public string Version { get; } = $"v{typeof(App).Assembly.GetName().Version.ToString(3)}";

        public string UpdateStatus
        {
            get => _updateStatus;
            private set => SetProperty(ref _updateStatus, value ?? string.Empty, nameof(UpdateStatus));
        }

        private string _updateStatus;

        public bool CanEdit
        {
            get => _canEdit;
            private set => SetProperty(ref _canEdit, value, nameof(CanEdit));
        }

        private bool _canEdit;

        public ObservableCollection<World> WorldList { get; }

        public World SelectedWorld
        {
            get => _selectedWorld;
            set => SetProperty(ref _selectedWorld, value, nameof(SelectedWorld));
        }

        private World _selectedWorld;

        public string GuildName
        {
            get => _guildName;
            set => SetProperty(ref _guildName, value ?? string.Empty, nameof(GuildName));
        }

        private string _guildName = string.Empty;

        public string GuildNameCheck
        {
            get => _guildNameCheck;
            set => SetProperty(ref _guildNameCheck, value ?? string.Empty, nameof(GuildNameCheck));
        }

        private string _guildNameCheck = string.Empty;

        public DelegateCommand SearchCommand { get; }

        public MainWindowViewModel()
        {
            WorldList = new ObservableCollection<World>
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

            SearchCommand = new DelegateCommand(ExecuteSearchCommand, CanExecuteSearchCommand);

            UpdateStatus = LocalizationString.updatenotify_check_update;
            PropertyChanged += MainWindowViewModel_PropertyChanged;

            CanEdit = true;
        }

        private void MainWindowViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GuildName))
            {
                bool valid = IsValidGuildName(GuildName);
                GuildNameCheck = valid ? string.Empty : LocalizationString.input_wrong_guild_name;
            }
        }

        private bool IsValidGuildName(string guildName)
        {
            if (!Regex.IsMatch(guildName, "^[0-9a-zA-Z가-힣]*$")) // 특수문자 입력 금지
            {
                return false;
            }

            float count = guildName.ToCharArray()
                                   .Sum(ch => Regex.IsMatch(ch.ToString(), "[0-9a-zA-Z]") ? 0.5f : 1f);

            return 2 <= count && count <= 6;
        }

        private bool CanExecuteSearchCommand(object _) =>
            SelectedWorld != null && GuildName.Length != 0 && GuildNameCheck.Length == 0 && CanEdit;

        private async void ExecuteSearchCommand(object _)
        {
            CanEdit = false;
            MessageBox.Show("Pressed");
            await Task.Delay(5000);
            CanEdit = true;
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
