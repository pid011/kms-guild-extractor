using System.Windows.Controls;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.ViewModel;

namespace KMSGuildExtractor.View
{
    /// <summary>
    /// Interaction logic for ExtractView.xaml
    /// </summary>
    public partial class ExtractView : UserControl
    {
        public ExtractView(Guild guildData)
        {
            InitializeComponent();
            DataContext = new ExtractViewModel(guildData);
        }
    }
}
