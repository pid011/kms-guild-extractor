using System;
using System.Collections.Generic;
using System.Text;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class InfoViewModel : BindableBase
    {
        public string Title => $"{LocalizationString.title} {Version}";

        public string Version => $"v{typeof(App).Assembly.GetName().Version.ToString(3)}";
    }
}
