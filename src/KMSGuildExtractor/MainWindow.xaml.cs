using System;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Navigation;

using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            string version = $"v{typeof(App).Assembly.GetName().Version.ToString(3)}";
            Title = $"{LocalizationString.title} {version}";
            InfoTitleTextBlock.Text = $"{LocalizationString.title}";
            VersionTextBlock.Text = version;
            // 버전확인해서 GitHubReleaseHyperLink.NavigateUri를 현재 버전의 링크로 수정하기

        }

        private void OpenHyperLink(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
            }
            catch (Win32Exception ex)
            {
                if (ex.ErrorCode == -2147467259)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void TitleBarMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
            }
        }

        private void CloseButtonClicked(object sender, RoutedEventArgs e) => Application.Current.Shutdown();

        private void MinimizeButtonClicked(object sender, RoutedEventArgs e) => WindowState = WindowState.Minimized;
    }
}
