using System;
using System.Diagnostics;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using HTWAppObjects;

namespace BackgroundTasks
{
    public sealed class TimetableBackgroundTask : IBackgroundTask
    {
        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            // update live tile
            try {
                // Load the item.
                XmlDocument tileXml = await TimetableModel.GetNextLessonXml();
                if (tileXml == null)
                    throw new Exception("Timetable is empty.");
                // Update the live tile with the item.
                TimetableModel.UpdateTile(tileXml);
            }
            catch {
                Debug.WriteLine("Missing timetable item. First load the timetable.");
            }

            // Inform the system that the task is finished.
            deferral.Complete();
        }
    }
}
