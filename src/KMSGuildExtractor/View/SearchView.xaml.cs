using System.Windows.Controls;

using KMSGuildExtractor.ViewModel;

namespace KMSGuildExtractor.View
{
    /// <summary>
    /// Interaction logic for SearchPage.xaml
    /// </summary>
    public partial class SearchView : UserControl
    {
        public SearchView(MainWindowViewModel main)
        {
            InitializeComponent();
            DataContext = new SearchViewModel(main);
        }
    }
}
