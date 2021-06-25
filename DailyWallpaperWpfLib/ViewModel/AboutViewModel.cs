using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DailyWallpaperWpfLib.ViewModel
{
    public class AboutViewModel : ADR_Library.ViewModel.ViewModelBase
    {
        public AboutViewModel()
        {
            _componentVersions = new System.Collections.ObjectModel.ObservableCollection<ComponentVersionInfo>();
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

        public void AddVersionInfo(string name, string version)
        {
            foreach (var item in ComponentVersions)
            {
                if (item.Name == name)
                {
                    item.Version = version;
                    OnPropertyChanged("ComponentVersions");
                    return;
                }
            }
            ComponentVersionInfo info = new ComponentVersionInfo();
            info.Name = name;
            info.Version = version;
            ComponentVersions.Add(info);
        }

        private System.Collections.ObjectModel.ObservableCollection<ComponentVersionInfo> _componentVersions;

        public System.Collections.ObjectModel.ObservableCollection<ComponentVersionInfo> ComponentVersions
        {
            get
            {
                return _componentVersions;
            }
            set
            {
                _componentVersions = value;
                OnPropertyChanged("ComponentVersions");
            }
        }
    }
}
