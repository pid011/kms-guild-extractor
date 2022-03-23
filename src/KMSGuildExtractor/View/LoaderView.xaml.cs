using System.Windows.Controls;

using KMSGuildExtractor.Core;
using KMSGuildExtractor.ViewModel;

namespace KMSGuildExtractor.View
{
    /// <summary>
    /// Interaction logic for LoaderView.xaml
    /// </summary>
    public partial class LoaderView : UserControl
    {
        public LoaderView(Guild guildData)
        {
            InitializeComponent();
            DataContext = new LoaderViewModel(guildData);
        }
    }
}
