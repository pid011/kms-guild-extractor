using System.Windows.Controls;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.ViewModel;

namespace KMSGuildExtractor.View
{
    /// <summary>
    /// Interaction logic for DataLoadView.xaml
    /// </summary>
    public partial class DataLoadView : UserControl
    {
        public DataLoadView(MainWindowViewModel main, Guild guildData)
        {
            InitializeComponent();
            DataContext = new DataLoadViewModel(main, guildData);
        }
    }
}
