using System;
using Template10.Mvvm;

namespace HTWDDAppUniversal.ViewModels
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public AboutPartViewModel AboutPartViewModel { get; } = new AboutPartViewModel();
    }

    public class AboutPartViewModel : ViewModelBase
    {
        public Uri Logo => Windows.ApplicationModel.Package.Current.Logo;

        public string DisplayName => Windows.ApplicationModel.Package.Current.DisplayName;

        public string Publisher => Windows.ApplicationModel.Package.Current.PublisherDisplayName;

        public string Version {
            get {
                var v = Windows.ApplicationModel.Package.Current.Id.Version;
                return $"{v.Major}.{v.Minor}.{v.Build}.{v.Revision}";
            }
        }
    }
}

