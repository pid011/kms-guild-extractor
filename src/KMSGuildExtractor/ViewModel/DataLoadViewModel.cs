using System;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class DataLoadViewModel : BindableBase
    {
        public Visibility InitializeLoading
        {
            get => _initializeLoading;
            set => SetProperty(ref _initializeLoading, value, nameof(InitializeLoading));
        }

        public Visibility MemberListVisivility
        {
            get => _memberListVisivility;
            set => SetProperty(ref _memberListVisivility, value, nameof(MemberListVisivility));
        }

        public string GuildSummary
        {
            get => _guildSummary;
            set => SetProperty(ref _guildSummary, value, nameof(GuildSummary));
        }

        public ObservableCollection<ListViewData> MemberList { get; } = new ObservableCollection<ListViewData>();
        public bool CanLoad
        {
            get => _canLoad;
            set => _canLoad = SetProperty(ref _canLoad, value, nameof(CanLoad));
        }
        public bool CanExtract
        {
            get => _canExtract;
            set => SetProperty(ref _canExtract, value, nameof(CanExtract));
        }

        public DelegateCommand LoadCommand { get; }
        public DelegateCommand ExtractCommand { get; }

        private readonly Guild _guild;

        private Visibility _initializeLoading;
        private Visibility _memberListVisivility;
        private string _guildSummary = string.Empty;
        private bool _canLoad;
        private bool _canExtract;

        private CancellationTokenSource _guildMemberLoadCancellation;
        //private ConcurrentQueue<(int number, User data)> _userWorkQueue;

        public DataLoadViewModel(Guild guildData)
        {
            _guild = guildData;
            GuildSummary = $"{_guild.Name}, {_guild.World.ToLocalizedString()}, {_guild.Level}Lv.";

            CanLoad = true;
            CanExtract = false;

            LoadCommand = new DelegateCommand(ExecuteLoadCommand);
            ExtractCommand = new DelegateCommand(ExecuteExtractCommand);

            _ = Task.Run(LoadGuildMember);
        }

        private async Task LoadGuildMember()
        {
            _guildMemberLoadCancellation = new CancellationTokenSource();
            try
            {
                MemberListVisivility = Visibility.Collapsed;
                InitializeLoading = Visibility.Visible;

                await _guild.LoadGuildMembersAsync(_guildMemberLoadCancellation.Token);

                InitializeLoading = Visibility.Collapsed;
                MemberListVisivility = Visibility.Visible;

                foreach ((GuildPosition position, User user) in _guild.Members)
                {
                    await Application.Current.Dispatcher.InvokeAsync(() => MemberList.Add(new ListViewData
                    {
                        Name = user.Name,
                        Position = position.ToLocalizedString()
                    }.SetStatus(ListViewData.State.Ready)
                    ));
                }
            }
            catch (ParseException)
            {
                return;
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }

        private void ExecuteExtractCommand(object _)
        {
            throw new NotImplementedException();
        }

        private void ExecuteLoadCommand(object _)
        {
            throw new NotImplementedException();
        }

        public class ListViewData
        {
            public enum State
            {
                Ready, Syncing, Loading, Done
            }

            public string Name { get; set; }
            public string Position { get; set; }
            public string Status { get; set; }
            public Color StatusColor { get; set; }

            public ListViewData SetStatus(State status)
            {
                // 준비됨                 Colors.Orange
                // 데이터를 동기화하는 중   Colors.LightSkyBlue
                // 데이터를 불러오는 중     Colors.LightSteelBlue
                // 완료                   Colors.LightGreen

                switch (status)
                {
                    case State.Ready:
                        Status = LocalizationString.load_ready;
                        StatusColor = Colors.Orange;
                        break;
                    case State.Syncing:
                        Status = LocalizationString.load_syncing;
                        StatusColor = Colors.LightSkyBlue;
                        break;
                    case State.Loading:
                        Status = LocalizationString.load_working;
                        StatusColor = Colors.LightSteelBlue;
                        break;
                    case State.Done:
                        Status = LocalizationString.load_done;
                        StatusColor = Colors.LightGreen;
                        break;
                }

                return this;
            }
        }
    }
}
