using System;
using System.Collections.Generic;
using System.Text;

using KMSGuildExtractor.Core;

namespace KMSGuildExtractor.ViewModel
{
    public class DataLoadViewModel : BindableBase
    {
        private readonly MainWindowViewModel _main;
        private readonly Guild _guild;

        public DataLoadViewModel(MainWindowViewModel main, Guild guildData)
        {
            _main = main;
            _guild = guildData;
        }
    }
}
