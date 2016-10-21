using Windows.UI.Xaml;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Template10.Controls;
using Windows.UI.Xaml.Data;
using HTWDDAppUniversal.Classes;

namespace HTWDDAppUniversal
{
    /// Documentation on APIs used in this page:
    /// https://github.com/Windows-XAML/Template10/wiki

    [Bindable]
    sealed partial class App : Template10.Common.BootStrapper
    {
        public App()
        {
            InitializeComponent();
            SplashFactory = (e) => new Views.Splash(e);

            #region App settings

            #endregion
        }

        public override async Task OnInitializeAsync(IActivatedEventArgs args)
        {
            if (Window.Current.Content as ModalDialog == null) {
                // create a new frame 
                var nav = NavigationServiceFactory(BackButton.Attach, ExistingContent.Include);

                // create modal root
                Window.Current.Content = new ModalDialog {
                    DisableBackButtonWhenModal = true,
                    Content = new Views.Shell(nav)
                };
            }
            await Task.CompletedTask;
        }

        public override async Task OnStartAsync(StartKind startKind, IActivatedEventArgs args)
        {
            // register background tasks
            // background task to update next lesson in tile
            BackgroundTaskHelper.RegisterBackgroundTask("TimetableBackgroundTask", "BackgroundTasks.TimetableBackgroundTask", 15);

            NavigationService.Navigate(typeof(Views.TimetablePage));
            await Task.CompletedTask;
        }
    }
}

