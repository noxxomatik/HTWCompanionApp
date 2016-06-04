using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;

namespace HTWAppUniversal.Classes {
    class TileBackgroundTask {
        private static TileBackgroundTask instance;
        private const string taskName = "TimetableBackgroundTask";
        private const string taskEntryPoint = "BackgroundTasks.TimetableBackgroundTask";

        private TileBackgroundTask() {
            this.RegisterBackgroundTask();
        }

        public static TileBackgroundTask getInstance () {
            if (instance == null)
                instance = new TileBackgroundTask();
            return instance;
        }

        public async void RegisterBackgroundTask() {
            var backgroundAccessStatus = await BackgroundExecutionManager.RequestAccessAsync();
            if (backgroundAccessStatus == BackgroundAccessStatus.AllowedMayUseActiveRealTimeConnectivity ||
                backgroundAccessStatus == BackgroundAccessStatus.AllowedWithAlwaysOnRealTimeConnectivity) {
                foreach (var task in BackgroundTaskRegistration.AllTasks) {
                    if (task.Value.Name == taskName) {
                        task.Value.Unregister(true);
                    }
                }

                BackgroundTaskBuilder taskBuilder = new BackgroundTaskBuilder();
                taskBuilder.Name = taskName;
                taskBuilder.TaskEntryPoint = taskEntryPoint;
                taskBuilder.SetTrigger(new TimeTrigger(15, false));
                var registration = taskBuilder.Register();
            }
        }
    }
}
