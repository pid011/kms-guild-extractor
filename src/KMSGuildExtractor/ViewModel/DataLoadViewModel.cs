using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

using KMSGuildExtractor.Core;

namespace KMSGuildExtractor.ViewModel
{
    public class DataLoadViewModel : BindableBase
    {
        public Visibility InitializeLoading
        {
            get => _initializeLoading;
            set => SetProperty(ref _initializeLoading, value, nameof(InitializeLoading));
        }

        private readonly MainWindowViewModel _main;
        private readonly Guild _guild;

        private Visibility _initializeLoading;
        private CancellationTokenSource _guildMemberLoadCancellation;

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
                InitializeLoading = Visibility.Visible;
                await _guild.LoadGuildMembersAsync(_guildMemberLoadCancellation.Token);
                InitializeLoading = Visibility.Collapsed;
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
    }
}
