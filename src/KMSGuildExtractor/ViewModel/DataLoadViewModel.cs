using System;
using System.Collections.Concurrent;
using System.Linq;
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
        private enum State
        {
            GettingMemberList, GettingMemberdata, Done, Error
        }

        public Visibility LoadingVisibility
        {
            get => _loadingVisibility;
            set => SetProperty(ref _loadingVisibility, value, nameof(LoadingVisibility));
        }

        public Visibility DoneVisibility
        {
            get => _doneVisibility;
            set => SetProperty(ref _doneVisibility, value, nameof(DoneVisibility));
        }

        public Visibility ErrorVisibility
        {
            get => _errorVisibility;
            set => SetProperty(ref _errorVisibility, value, nameof(ErrorVisibility));
        }

        public string StateMessage
        {
            get => _stateMessage;
            set => SetProperty(ref _stateMessage, value, nameof(StateMessage));
        }

        public Brush StateColor
        {
            get => _stateColor;
            set => SetProperty(ref _stateColor, value, nameof(StateColor));
        }

        public bool CanExtract
        {
            get => _canExtract;
            set => SetProperty(ref _canExtract, value, nameof(CanExtract));
        }

        public DelegateCommand ExtractCommand { get; }

        private readonly Guild _guild;

        private Visibility _loadingVisibility;
        private Visibility _errorVisibility;
        private Visibility _doneVisibility;
        private bool _canExtract;
        private string _stateMessage = string.Empty;
        private Brush _stateColor;

        private CancellationTokenSource _taskCancellation;

        public DataLoadViewModel(Guild guildData)
        {
            _guild = guildData;
            CanExtract = false;
            ExtractCommand = new DelegateCommand(ExecuteExtractCommand);
            Task.Run(LoadData);
        }

        private async Task LoadData()
        {
            int max;
            int count = 0;
            int errorCount = 0;

            ConcurrentQueue<int> memberIndex;

            try
            {
                _taskCancellation = new CancellationTokenSource();

                StateMessage = LocalizationString.state_get_members;
                SetState(State.GettingMemberList);

                await _guild.LoadGuildMembersAsync(_taskCancellation.Token);

                max = _guild.Members.Count;
                memberIndex = new ConcurrentQueue<int>(Enumerable.Range(0, _guild.Members.Count));

                StateMessage = string.Format(LocalizationString.state_get_data, max, 0);
                SetState(State.GettingMemberdata);

                Task loadTask1 = Load();
                Task loadTask2 = Load();
                Task loadTask3 = Load();

                await loadTask1;
                await loadTask2;
                await loadTask3;
            }
            catch (TaskCanceledException)
            {
                return;
            }
            catch (Exception)
            {
                errorCount++;
            }
            finally
            {
                if (errorCount == 0)
                {
                    StateMessage = LocalizationString.state_done;
                    SetState(State.Done);

                    CanExtract = true;
                }
                else
                {
                    StateMessage = $"Error Count: {errorCount}";
                    SetState(State.Error);
                    CanExtract = false;
                }
            }

            async Task Load()
            {
                while (memberIndex.TryDequeue(out int idx))
                {
                    try
                    {
                        await _guild.Members[idx].data.RequestSyncAsync(_taskCancellation.Token);
                        await _guild.Members[idx].data.LoadUserDetailAsync(_taskCancellation.Token);
                    }
                    catch (Exception)
                    {
                        errorCount++;
                    }
                    finally
                    {
                        count++;
                        StateMessage = string.Format(LocalizationString.state_get_data, max, count);
                        await Task.Delay(2000);
                    }
                }
            }
        }

        private void ExecuteExtractCommand(object _)
        {
            MessageBox.Show("Extracted");
        }

        private void SetState(State state)
        {
            StateColor = GetStateColor(state);
            switch (state)
            {
                case State.GettingMemberList:
                    DoneVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Visible;
                    break;
                case State.GettingMemberdata:
                    DoneVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Visible;
                    break;
                case State.Done:
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Collapsed;
                    DoneVisibility = Visibility.Visible;
                    break;
                case State.Error:
                    DoneVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Collapsed;
                    ErrorVisibility = Visibility.Visible;
                    break;
            }
        }

        private Brush GetStateColor(State state) => state switch
        {
            State.GettingMemberList => Brushes.Orange,

            State.GettingMemberdata => Brushes.LightSkyBlue,

            State.Done => Brushes.LightGreen,

            State.Error => Brushes.Red,

            _ => Brushes.Transparent,
        };
    }
}
