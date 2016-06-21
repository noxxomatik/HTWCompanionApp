using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using HTWAppObjects;
using NotificationsExtensions.Toasts;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    public sealed class TimetableBackgroundTask : IBackgroundTask {
        public async void Run(IBackgroundTaskInstance taskInstance) {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // update live tile
            try {
                // Load the item.
                XmlDocument tileXml = await TimetableObject.GetNextLessonXml();
                if (tileXml == null)
                    throw new Exception("Timetable is empty.");
                // Update the live tile with the item.
                TimetableObject.UpdateTile(tileXml);
            }
            catch () {
                Debug.WriteLine("Missing timetable item. First load the timetable.");
            }

            // check for new grades
            try {
                GradesModel gm = GradesModel.getInstance();
                SettingsModel sm = SettingsModel.getInstance();
                int countNewGrades = await gm.getNewGradesCount(sm.SNummer, sm.RZLogin);
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
            catch () {
                Debug.WriteLine("Grades could not be retrieved.");
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        
    }
}
