using System;
using System.Collections.Generic;
using System.Text;
using KMSGuildExtractor.Localization;

namespace KMSGuildExtractor.ViewModel
{
    public class UpdateNotifyViewModel : BindableBase
    {
        public string UpdateStatus
        {
            get => _updateStatus;
            private set => SetProperty(ref _updateStatus, value);
        }
        private string _updateStatus;

        public UpdateNotifyViewModel()
        {
            UpdateStatus = LocalizationString.updatenotify_check_update;
        }
    }
}
