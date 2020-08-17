using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

using KMSGuildExtractor.Core;

namespace KMSGuildExtractor.ViewModel
{
    internal class DataLoadViewModel : BindableBase
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

        public ObservableCollection<ListViewData> MemberList { get; } = new ObservableCollection<ListViewData>();

        private readonly MainWindowViewModel _main;
        private readonly Guild _guild;

        private Visibility _initializeLoading;
        private Visibility _memberListVisivility;

        private CancellationTokenSource _guildMemberLoadCancellation;
        private ConcurrentQueue<(int number, User data)> _userWorkQueue;

        public DataLoadViewModel(MainWindowViewModel main, Guild guildData)
        {
            _main = main;
            _guild = guildData;

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
                        Position = position.ToLocalizedString(),
                        Status = "준비됨"
                    }));
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

        internal class ListViewData
        {
            public string Name { get; set; }
            public string Position { get; set; }
            public string Status { get; set; }
        }
    }
}
