using System.Collections.ObjectModel;
using System.ComponentModel;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class SearchViewModel : BindableBase
    {
        private readonly MainWindowViewModel _main;

        public bool CanEdit
        {
            get => _canEdit;
            private set => SetProperty(ref _canEdit, value, nameof(CanEdit));
        }

        private bool _canEdit;

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

        public SearchViewModel(MainWindowViewModel main)
        {
            _main = main;
            SearchCommand = new DelegateCommand(ExecuteSearchCommand, CanExecuteSearchCommand);
            CanEdit = true;

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(GuildName))
            {
                bool valid = Guild.IsValidGuildName(GuildName);
                GuildNameCheck = valid ? string.Empty : LocalizationString.input_wrong_guild_name;
            }
        }

        private bool CanExecuteSearchCommand(object _) =>
            CanEdit && SelectedWorld != null && GuildName.Length != 0 && GuildNameCheck.Length == 0;

        private void ExecuteSearchCommand(object _)
        {
            //_main.WorkView = new TestView(_main, GuildName);
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
