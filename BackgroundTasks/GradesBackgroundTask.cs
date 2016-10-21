using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using HTWAppObjects;
using Windows.UI.Notifications;
using NotificationsExtensions.Toasts;

namespace BackgroundTasks
{
    public sealed class GradesBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // check for new grades
            try {
                GradesModel gradesModel = GradesModel.GetInstance();
                SettingsModel settingsModel = SettingsModel.GetInstance();
                int countNewGrades = await gradesModel.GetNewGradesCount(settingsModel.SNummer, settingsModel.RZLogin);
                if (countNewGrades > 0) {
                    ToastContent content = new ToastContent() {
                        Visual = new ToastVisual() {
                            TitleText = new ToastText() {
                                Text = countNewGrades + " neue Noten verfügbar!"
                            }
                        },
                        Audio = new ToastAudio() {
                            Src = new Uri("ms-winsoundevent:Notification.IM")
                        }
                    };
                    XmlDocument xmlContent = content.GetXml();
                    var toast = new ToastNotification(xmlContent);
                    ToastNotificationManager.CreateToastNotifier().Show(toast);
                }
            }
            catch {
                Debug.WriteLine("Grades could not be retrieved.");
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }
    }
}
