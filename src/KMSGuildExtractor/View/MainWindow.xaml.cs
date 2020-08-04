using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;

using KMSGuildExtractor.Localization;
using KMSGuildExtractor.ViewModel;

namespace KMSGuildExtractor.View
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        // public MainWindowViewModel MainWindowViewModel { get; } = new MainWindowViewModel();

        public MainWindow()
        {
            InitializeComponent();
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
