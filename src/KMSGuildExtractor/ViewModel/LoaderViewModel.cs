using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class LoaderViewModel : BindableBase
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

        public LoaderViewModel(Guild guildData)
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

                List<Task> loadTaskes = new()
                {
                    LoadAsync(1),
                    LoadAsync(2)
                };

                await Task.WhenAll(loadTaskes);

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
                var prefix = $"[task.{taskNumber}] ";

                while (memberIndex.TryDequeue(out int idx))
                {
                    var userName = _guild.Members[idx].data.Name;
                    try
                    {
                        Log.Information($"{prefix}Syncing member n.{idx} - name: {userName}");

                        var syncRequestCount = 0;
                        while (true)
                        {
                            try
                            {
                                syncRequestCount++;
                                await _guild.Members[idx].data.RequestSyncAsync(new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token);
                            }
                            catch (UserSyncException e) when (e.RawJsonValue != null)
                            {
                                Log.Error(e, $"{prefix}Faild to parse sync data. user name: {userName}\n[Raw json data]\n{e.RawJsonValue}");
                                return;
                            }
                            catch (UserSyncException e)
                            {
                                Debug.WriteLine($"{prefix}Faild to sync user data. ({syncRequestCount}) user name: {userName}");
                                Log.Warning(e, $"{prefix}Faild to sync user data. ({syncRequestCount}) user name: {userName}");
                                if (syncRequestCount > 6)
                                {
                                    return;
                                }

                                Log.Information("Try again in 10 seconds.");
                                await Task.Delay(TimeSpan.FromSeconds(10));
                                continue;
                            }
                            break;
                        }

                        Log.Information($"{prefix}Successfuly synced member n.{idx}");

                        Log.Information($"{prefix}Getting member details n.{idx} - name: {userName}");
                        await _guild.Members[idx].data.LoadUserDetailAsync();
                        Log.Information($"{prefix}Successfuly getting member details n.{idx}");
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (UserNotFoundException e)
                    {
                        Log.Error(e, $"{prefix}User {userName} doesn't found in maple.gg.");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e, $"{prefix}Faild to get user data. user name: {userName}");
                    }
                    finally
                    {
                        count++;
                        string msg = string.Format(LocalizationString.state_get_data, max, count);
                        Log.Information(msg);
                        StateMessage = msg;
                        await Task.Delay(1000);
                    }
                }

                Log.Information($"{prefix}Done.");
            }
        }

        private async void ExecuteExtractCommand(object _)
        {
            CanExtract = false;

            try
            {
                SaveFileDialog dialog = new()
                {
                    Filter = "CSV file (*.csv)|*.csv",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (dialog.ShowDialog() ?? false)
                {
                    await DataExtract.CreateCSVAsync(dialog.FileName, _guild);

                    var dir = Path.GetDirectoryName(dialog.FileName);
                    Process.Start("explorer.exe", dir);
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

        private static Brush GetStateColor(State state) => state switch
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
