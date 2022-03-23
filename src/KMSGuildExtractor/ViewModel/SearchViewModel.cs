using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;
using KMSGuildExtractor.View;

using Serilog;

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

            new World(WorldID.Scania),
            new World(WorldID.Bera),
            new World(WorldID.Luna),
            new World(WorldID.Zenith),
            new World(WorldID.Croa),
            new World(WorldID.Union),
            new World(WorldID.Elysium),
            new World(WorldID.Enosis),
            new World(WorldID.Red),
            new World(WorldID.Aurora),
            new World(WorldID.Nova),
            new World(WorldID.Arcane),
            new World(WorldID.Reboot),
            new World(WorldID.Reboot2),
            new World(WorldID.Burning),
            new World(WorldID.Burning2)
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
        private Guild _searchResult;

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
                if (SelectedWorld is null)
                {
                    done = false;
                    return;
                }

                SetSearchResultMessageSingle(LocalizationString.search_ing);

                _searchResult = await Guild.SearchAsync(InputGuildName, SelectedWorld.Url);

                if (_searchResult is null)
                {
                    SetSearchResultMessageSingle(LocalizationString.search_no_result);
                }
                else
                {
                    SetSearchResultMessage(InputGuildName, SelectedWorld.Name, $"{_searchResult.Level}Lv.");
                    done = true;
                }
            }
            catch (TaskCanceledException)
            {
                SetSearchResultMessageSingle(LocalizationString.search_no_result);
                done = false;
            }
            catch (ParseException e)
            {
                SetSearchResultMessageSingle(LocalizationString.search_error);
                done = false;
                Log.Error(e.ToString());
            }
            finally
            {
                CanSubmit = done;

                await Task.Delay(1000);
                CanEdit = true;
                CanSearch = true;
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
            if (_searchResult is null)
            {
                return;
            }
            _main.WorkView = new LoaderView(_searchResult);
        }

        public class World
        {
            public string Name { get; }

            public Uri WorldLogoPath =>
                new($"pack://application:,,,/resources/icons/worlds/{Url.ToString().ToLower()}.png");

            public WorldID Url { get; }

            public World(WorldID url)
            {
                Name = url.ToLocalizedString();
                Url = url;
            }
        }
    }
}
