using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace DailyWallpaperWpfLib.ViewModel
{
    public class StatusViewModel : ADR_Library.ViewModel.ViewModelBase
    {
        public StatusViewModel()
        {
            StatusFlags = new System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string,string>>();
        }

        private System.Windows.Media.ImageSource _icon;

        public System.Windows.Media.ImageSource Icon
        {
            get
            {
                return _icon;
            }
            set
            {
                _icon = value;
                OnPropertyChanged("Icon");
            }
        }

        private bool _isRunning = false;

        public bool IsRunning
        {
            get
            {
                return _isRunning;
            }
            set
            {
                _isRunning = value;
                OnPropertyChanged("IsRunning");
            }
        }

        private System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string,string>> _statusFlags;

        public System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string, string>> StatusFlags
        {
            get
            {
                return _statusFlags;
            }
            set
            {
                _statusFlags = value;
                OnPropertyChanged("StatusFlags");
            }
        }

        public void SetStatusFlags(List<KeyValuePair<string, string>> flags)
        {
            StatusFlags = new System.Collections.ObjectModel.ObservableCollection<KeyValuePair<string, string>>(flags);
        }
    }
}
