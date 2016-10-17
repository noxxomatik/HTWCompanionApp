using Windows.UI.Xaml;
using System.Threading.Tasks;
using HTWDDAppUniversal.Services.SettingsServices;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Windows.UI.Xaml.Data;
using HTWDDAppUniversal.Classes;
using Windows.UI.ViewManagement;
using Windows.Foundation.Metadata;
using Windows.UI;

namespace HTWDDAppUniversal
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App() {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            var _settings = SettingsService.Instance;
            RequestedTheme = _settings.AppTheme;
            CacheMaxDuration = _settings.CacheMaxDuration;
            ShowShellBackButton = _settings.UseShellBackButton;

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args) {
            if (Window.Current.Content as ModalDialog == null) {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav),
                    ModalContent = new Views.Busy(),
                };
            }
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args) {
            // register background tasks
            BackgroundTaskHelper helper = BackgroundTaskHelper.getInstance();
            // background task to update next lesson in tile
            helper.RegisterBackgroundTask("TimetableBackgroundTask", "BackgroundTasks.TimetableBackgroundTask", 15);
            // background task that checks for new grades
            helper.RegisterBackgroundTask("GradesBackgroundTask", "BackgroundTasks.GradesBackgroundTask", 60);

            // hide status bar
            if (ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar")) {
                //StatusBar statusBar = StatusBar.GetForCurrentView();
                //statusBar.HideAsync();
            }

            NavigationService.Navigate(typeof(Views.TimetablePage));
            await Task.CompletedTask;
        }
    }
}

