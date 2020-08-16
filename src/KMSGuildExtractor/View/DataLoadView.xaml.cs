using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
