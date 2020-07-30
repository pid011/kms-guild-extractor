using System;
using System.Diagnostics;
using System.Windows;
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
        }

        private void OpenHyperLink(object sender, RequestNavigateEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo(e.Uri.ToString()) { UseShellExecute = true });
            }
            catch (System.ComponentModel.Win32Exception ex)
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
    }
}
