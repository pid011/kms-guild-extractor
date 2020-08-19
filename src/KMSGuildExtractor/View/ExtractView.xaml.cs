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
        public DataLoadView(Guild guildData)
        {
            InitializeComponent();
            DataContext = new ExtractViewModel(guildData);
        }
    }
}
