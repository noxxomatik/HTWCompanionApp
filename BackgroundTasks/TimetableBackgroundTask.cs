using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;
using Windows.Web.Syndication;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

using Windows.Storage;
using NotificationsExtensions.Tiles;
using NotificationsExtensions.Toasts;
using HTWAppObjects;
using System.Globalization;

namespace BackgroundTasks {
    public sealed class TimetableBackgroundTask : IBackgroundTask {
        public async void Run(IBackgroundTaskInstance taskInstance) {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try {
                // Load the item.
                XmlDocument tileXml = await TimetableObject.GetNextLessonXml();
                if (tileXml == null)
                    throw new Exception("Timetable is empty.");
                // Update the live tile with the item.
                TimetableObject.UpdateTile(tileXml);
            }
            catch (Exception e) {
                Debug.WriteLine("Missing timteable item. First load the timetable.");
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }
    }
}
