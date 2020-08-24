using System;
using System.Collections.Concurrent;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.Localization;

using Microsoft.Win32;

using Serilog;

namespace KMSGuildExtractor.ViewModel
{
    public class ExtractViewModel : BindableBase
    {
        private enum State
        {
            Ready, GettingMemberList, GettingMemberdata, Done, Error
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
        private Brush _stateColor = Brushes.Transparent;

        public ExtractViewModel(Guild guildData)
        {
            _guild = guildData;
            CanExtract = false;
            SetState(State.Ready);
            StateMessage = string.Empty;
            ExtractCommand = new DelegateCommand(ExecuteExtractCommand);
            Task.Run(LoadDataAsync);
        }

        private async Task LoadDataAsync()
        {
            int max;
            int count = 0;
            int errorCount = 0;

            ConcurrentQueue<int> memberIndex;

            Log.Information("LoadData - guild name: {GuildName}, guild id: {GuildID}", _guild.Name, _guild.GuildID);

            try
            {
                StateMessage = LocalizationString.state_get_members;
                SetState(State.GettingMemberList);

                Log.Information("Getting guild members...");
                await _guild.LoadGuildMembersAsync();
                Log.Information("Done. Member count: {Count}", _guild.Members.Count);
                max = _guild.Members.Count;
            }
            catch (Exception e)
            {
                Log.Error(e, "Faild to get guild members. guild name: {GuildName}", _guild.Name);
                return;
            }

            try
            {
                memberIndex = new ConcurrentQueue<int>(Enumerable.Range(0, _guild.Members.Count));

                StateMessage = string.Format(LocalizationString.state_get_data, max, 0);
                SetState(State.GettingMemberdata);

                Task loadTask1 = LoadAsync(1);
                Task loadTask2 = LoadAsync(2);
                Task loadTask3 = LoadAsync(3);

                await loadTask1;
                await loadTask2;
                await loadTask3;

                Log.Information("Load Task completed.");

                StateMessage = LocalizationString.state_done;
                SetState(State.Done);

                CanExtract = true;
            }
            catch (TaskCanceledException)
            {
                Log.Information("Load Task canceled.");

                StateMessage = LocalizationString.state_canceled;
                SetState(State.Error);

                CanExtract = false;
            }
            finally
            {
                Log.Information("Error Count: {Count}", errorCount);
            }

            async Task LoadAsync(int taskNumber)
            {
                while (memberIndex.TryDequeue(out int idx))
                {
                    try
                    {
                        Log.Information("[task.{TaskNumber}] Syncing member n.{Index} - name: {Name}", taskNumber, idx, _guild.Members[idx].data.Name);
                        await _guild.Members[idx].data.RequestSyncAsync(new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token);
                        Log.Information("[task.{TaskNumber}] Successfuly synced member n.{Index}", taskNumber, idx);

                        Log.Information("[task.{TaskNumber}] Getting member details n.{Index} - name: {Name}", taskNumber, idx, _guild.Members[idx].data.Name);
                        await _guild.Members[idx].data.LoadUserDetailAsync();
                        Log.Information("[task.{TaskNumber}] Successfuly getting member details n.{Index}", taskNumber, idx);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (UserSyncException e) when (e.RawJsonValue != null)
                    {
                        Log.Error(e, "[task.{TaskNumber}] Faild to parse sync data. user name: {UserName}\n[Raw json data]\n{Json}", taskNumber, _guild.Members[idx].data.Name, e.RawJsonValue);
                    }
                    catch (UserSyncException e)
                    {
                        Log.Error(e, "[task.{TaskNumber}] Faild to sync user data. user name: {UserName}", taskNumber, _guild.Members[idx].data.Name);
                    }
                    catch (UserNotFoundException e)
                    {
                        Log.Error(e, "[task.{TaskNumber}] User {UserName} doesn't found in maple.gg.", taskNumber, _guild.Members[idx].data.Name);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, "[task{TaskNumber}] Faild to get user data. user name: {UserName}", taskNumber, _guild.Members[idx].data.Name);
                    }
                    finally
                    {
                        count++;
                        string msg = string.Format(LocalizationString.state_get_data, max, count);
                        Log.Information(msg);
                        StateMessage = msg;
                        await Task.Delay(2000);
                    }
                }

                Log.Information("[task.{TaskNumber}] Done.", taskNumber);
            }
        }

        private async void ExecuteExtractCommand(object _)
        {
            CanExtract = false;

            try
            {
                SaveFileDialog dialog = new SaveFileDialog
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dialog.ShowDialog() ?? false)
                {
                    await DataExtract.CreateCSVAsync(dialog.FileName, _guild);
                }
            }
            catch (IOException e)
            {
                Log.Error(e, "File IO Exception");
                MessageBox.Show(e.Message);
            }
            catch (Exception e)
            {
                Log.Error(e, "Faild to extract data to csv");
            }
            finally
            {
                CanExtract = true;
            }
        }

        private void SetState(State state)
        {
            StateColor = GetStateColor(state);
            switch (state)
            {
                case State.Ready:
                    ErrorVisibility = Visibility.Collapsed;
                    LoadingVisibility = Visibility.Collapsed;
                    DoneVisibility = Visibility.Visible;
                    break;
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
            State.Ready => Brushes.Gray,

            State.GettingMemberList => Brushes.Orange,

            State.GettingMemberdata => Brushes.LightSkyBlue,

            State.Done => Brushes.LightGreen,

            State.Error => Brushes.Red,

            _ => Brushes.Transparent,
        };
    }
}
