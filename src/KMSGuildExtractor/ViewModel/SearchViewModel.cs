using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class SearchViewModel : BindableBase
    {
        public bool CanEdit
        {
            get => _canEdit;
            private set => SetProperty(ref _canEdit, value, nameof(CanEdit));
        }

        public bool CanSearch
        {
            get => _canSearch;
            private set => SetProperty(ref _canSearch, value, nameof(CanSearch));
        }

        public bool CanSubmit
        {
            get => _canSubmit;
            private set => SetProperty(ref _canSubmit, value, nameof(CanSubmit));
        }

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
        public string InputGuildName
        {
            get => _inputGuildName;
            set => SetProperty(ref _inputGuildName, value ?? string.Empty, nameof(InputGuildName));
        }
        public string InputGuildNameCheck
        {
            get => _inputGuildNameCheck;
            set => SetProperty(ref _inputGuildNameCheck, value ?? string.Empty, nameof(InputGuildNameCheck));
        }

        public string SearchResultGuildName
        {
            get => _searchResultGuildName;
            set => SetProperty(ref _searchResultGuildName, value, nameof(SearchResultGuildName));
        }

        public string SearchResultGuildWorld
        {
            get => _searchResultGuildWorld;
            set => SetProperty(ref _searchResultGuildWorld, value, nameof(SearchResultGuildWorld));
        }

        public string SearchResultGuildLevel
        {
            get => _searchResultGuidLevel;
            set => SetProperty(ref _searchResultGuidLevel, value, nameof(SearchResultGuildLevel));
        }

        public DelegateCommand SearchCommand { get; }
        public DelegateCommand SubmitCommand { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
        private readonly MainWindowViewModel _main;

        private bool _canEdit;
        private bool _canSearch;
        private bool _canSubmit;
        private World _selectedWorld;
        private string _inputGuildName = string.Empty;
        private string _inputGuildNameCheck = string.Empty;
        private string _searchResultGuildName = LocalizationString.search_no_result;
        private string _searchResultGuildWorld = LocalizationString.search_no_result;
        private string _searchResultGuidLevel = LocalizationString.search_no_result;

        private CancellationTokenSource _searchCancellation;
        private IGuild _searchResult;

        public SearchViewModel(MainWindowViewModel main)
        {
            _main = main;
            SearchCommand = new DelegateCommand(ExecuteSearchCommand);
            SubmitCommand = new DelegateCommand(ExecuteSubmitCommand);
            CanEdit = true;
            CanSearch = false;
            CanSubmit = false;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(InputGuildName))
            {
                bool valid = Guild.IsValidGuildName(InputGuildName);
                InputGuildNameCheck = valid ? string.Empty : LocalizationString.input_wrong_guild_name;
                CanSearch = CanEdit && SelectedWorld != null && InputGuildName.Length != 0 && valid;
            }
        }

        private async void ExecuteSearchCommand(object _)
        {
            bool done = false;
            CanEdit = false;
            CanSearch = false;
            CanSubmit = false;
            try
            {
                SetSearchResultMessageSingle(LocalizationString.search_ing);

                _searchCancellation = new CancellationTokenSource();
                _searchResult = await Guild.SearchAsync(InputGuildName, SelectedWorld.Url, _searchCancellation.Token);

                if (_searchResult is null)
                {
                    SetSearchResultMessageSingle(LocalizationString.search_no_result);
                }
                else
                {
                    SetSearchResultMessage(InputGuildName, SelectedWorld.Name, _searchResult.Level.ToString());
                    done = true;
                }
            }
            catch (TaskCanceledException)
            {
                SetSearchResultMessageSingle(LocalizationString.search_no_result);
                done = false;
            }
            catch (ParseException)
            {
                SetSearchResultMessageSingle(LocalizationString.search_error);
                done = false;
                // Serilog로 Exception 데이터 저장하기
            }
            finally
            {
                CanEdit = true;

                await Task.Delay(1000);
                CanSearch = true;
                CanSubmit = done;
            }

            void SetSearchResultMessageSingle(string singleMessage)
            {
                SetSearchResultMessage(singleMessage, singleMessage, singleMessage);
            }

            void SetSearchResultMessage(string name, string world, string level)
            {
                SearchResultGuildName = name;
                SearchResultGuildWorld = world;
                SearchResultGuildLevel = level;
            }
        }

        private void ExecuteSubmitCommand(object _)
        {
            MessageBox.Show(_searchResult is null
                ? "search result is null"
                : $"{_searchResult.Name}, {_searchResult.World}, {_searchResult.Level}, {_searchResult.GuildID}");
            //_main.WorkView = new TestView(_main, GuildName);
        }

        public class World
        {
            public string Name { get; }

            public Uri WorldLogoPath =>
                new Uri($"pack://application:,,,/resources/icons/worlds/{Url.ToString().ToLower()}.png");

            public WorldID Url { get; }

            public World(string name, WorldID url)
            {
                Name = name;
                Url = url;
            }
        }

    }
}
