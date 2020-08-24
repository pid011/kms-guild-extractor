using System.IO;
using System.Windows;

using KMSGuildExtractor.Utils;

using Serilog;

namespace KMSGuildExtractor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.File(Path.Combine(ProgramPath.ProgramDirectoryPath, "KMSGuildExtrator_logs", ".log"),
                              rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }
    }
}
