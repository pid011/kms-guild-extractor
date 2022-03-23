using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
            Log.Information($"LoadData - GuildName: {_guild.Name}, GuildId: {_guild.GuildID}");

            DisplayLog(State.GettingMemberList, LocalizationString.state_get_members);
            try
            {
                Log.Information("Getting guild members...");
                await _guild.LoadGuildMembersAsync();
                Log.Information($"Done. Member count: {_guild.Members.Count}");
            }
            catch (Exception e)
            {
                Log.Error(e, $"Faild to get guild members. guild name: {_guild.Name}");
                DisplayLog(State.Error, LocalizationString.state_error);
                return;
            }

            DisplayLog(State.GettingMemberdata, string.Format(LocalizationString.state_get_data, _guild.Members.Count, 0));
            try
            {
                Log.Information("Start get user informations from maple.gg");

                /* maple.gg 서버 부담이 심한거 같아 동기화 기능 제외
                {
                    var count = 0;
                    var progress = new Progress<string>(name =>
                    {
                        ++count;
                        Log.Information($"Succefully synced user '{name}'");
                        DisplayLog(State.GettingMemberdata, $"'{name}' 유저 정보 업데이트 완료 ({count}/{_guild.Members.Count})");
                    });
                    await RequestInfoUpdateAsync(_guild, progress, default);
                }
                */
                {
                    var count = 0;
                    var progress = new Progress<string>(name =>
                    {
                        ++count;
                        // Log.Information($"Succefully get user information '{name}' ({count}/{_guild.Members.Count})");
                        DisplayLog(State.GettingMemberdata, $"'{name}' 유저 정보 가져오기 완료 ({count}/{_guild.Members.Count})");
                    });
                    await RequestMembersInfoAsync(_guild, progress, default);
                }

                Log.Information($"Done.");
                DisplayLog(State.Done, LocalizationString.state_done);
                CanExtract = true;
            }
            catch (TaskCanceledException)
            {
                Log.Information("Load Task canceled.");
                DisplayLog(State.Error, LocalizationString.state_canceled);
                CanExtract = false;
            }
            catch (Exception e)
            {
                Log.Error(e, "Faild to load member data!");
                DisplayLog(State.Error, LocalizationString.state_error);
                CanExtract = false;
            }
        }

        /* maple.gg 서버 부담이 심한거 같아 동기화 기능 제외
        private static async Task RequestInfoUpdateAsync(Guild guild, IProgress<string> progress, CancellationToken cancellation)
        {
            var tasks = new List<Task>();
            foreach (GuildMember member in guild.Members)
            {
                Debug.WriteLine($"Start sync process user '{member.Info.Name}'");

                Task task = RequestAsync(member, progress, cancellation);
                tasks.Add(task);
                await Task.Delay(TimeSpan.FromSeconds(5), cancellation);
            }
            await Task.WhenAll(tasks);

            static async Task RequestAsync(GuildMember member, IProgress<string> progress, CancellationToken cancellation)
            {
                string userName = member.Info.Name;
                User info = member.Info;

                int retryCount = 0;
                while (true)
                {
                    Log.Information($"[{userName}] Trying to request information sync to maple.gg");
                    try
                    {
                        await info.RequestSyncAsync(new CancellationTokenSource(TimeSpan.FromSeconds(60)).Token);
                    }
                    catch (UserSyncException e) when (e.RawJsonValue != null)
                    {
                        Log.Error(e, $"[{userName}] Faild to parse sync data.\n[Raw json data]\n{e.RawJsonValue}");
                        return;
                    }
                    catch (UserSyncException e)
                    {
                        Debug.WriteLine($"[{userName}] Faild to sync user data.");
                        Log.Warning(e, $"[{userName}] Faild to sync user data.");

                        if (retryCount < 5)
                        {
                            Log.Warning(e, $"[{userName}] Try again after 31 seconds.");
                            await Task.Delay(TimeSpan.FromSeconds(31), cancellation);
                            ++retryCount;
                            continue;
                        }

                        return;
                    }

                    break;
                }
                Log.Information($"[{userName}] Successfuly synced.");
                progress.Report(userName);
            }
        }
        */
        private static async Task RequestMembersInfoAsync(Guild guild, IProgress<string> progress, CancellationToken cancellation)
        {
            var tasks = new List<Task>();
            foreach (GuildMember member in guild.Members)
            {
                Debug.WriteLine($"[{member.Info.Name}] Start information load process");

                Task task = RequestAsync(member, progress, cancellation);
                tasks.Add(task);
                await Task.Delay(750, cancellation);
            }
            await Task.WhenAll(tasks);

            static async Task RequestAsync(GuildMember member, IProgress<string> progress, CancellationToken cancellation)
            {
                string userName = member.Info.Name;
                User info = member.Info;

                try
                {
                    await info.LoadUserDetailAsync(cancellation);
                }
                catch (UserNotFoundException)
                {
                    Log.Information($"[{userName}] Cannot found user in maple.gg");
                    return;
                }
                Log.Information($"[{userName}] Successfuly loaded.");
                progress.Report(userName);
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

        private void DisplayLog(State state, string text)
        {
            SetState(state);
            StateMessage = text;
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
