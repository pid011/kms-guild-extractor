using System;
using System.Collections.Generic;
using System.Text;

namespace KMSGuildExtractor.ViewModel
{
    public class MainWindowViewModel
    {
        public InfoViewModel InfoViewModel { get; } = new InfoViewModel();

        public UpdateNotifyViewModel UpdateNotifyViewModel { get; } = new UpdateNotifyViewModel();
    }
}
