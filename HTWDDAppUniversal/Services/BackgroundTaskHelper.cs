using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace HTWDDAppUniversal.Classes {
    class BackgroundTaskHelper {
        private static BackgroundTaskHelper instance;

        private BackgroundTaskHelper() {
        }

        public static BackgroundTaskHelper getInstance () {
            if (instance == null)
                instance = new BackgroundTaskHelper();
            return instance;
        }

        public async void RegisterBackgroundTask(string taskName, string taskEntryPoint, uint triggerIntervalMinutes) {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AlwaysAllowed ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedSubjectToSystemPolicy) {
                foreach (var task in BackgroundTaskRegistration.AllTasks) {
                    if (task.Value.Name == taskName) {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(triggerIntervalMinutes, false));
                var registration = taskBuilder.Register();
            }
        }
    }
}
