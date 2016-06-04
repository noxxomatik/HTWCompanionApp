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

namespace BackgroundTasks {
    public sealed class TimetableBackgroundTask : IBackgroundTask {
        private const string filename = "timetable.xml";

        public async void Run(IBackgroundTaskInstance taskInstance) {
            // Get a deferral, to prevent the task from closing prematurely 
            // while asynchronous code is still running.
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            try {
                // Load the item.
                XmlDocument tileXml = await GetNextLessonXml();

                // Update the live tile with the item.
                UpdateTile(tileXml);
            } catch (Exception e) {
                Debug.WriteLine("Missing timteable item. First load the timetable.");
            }            

            // Inform the system that the task is finished.
            deferral.Complete();
        }

        private async Task<XmlDocument> GetNextLessonXml() {
            List<TimetableObject> timetableObjects = await LoadTimetableBackup();

            // TODO: find the next lesson

            // build the tile
            TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive() {
                Children = {
                    new TileText() {
                        Text = "Nächste Stunde",
                        Style = TileTextStyle.CaptionSubtle
                    },
                    new TileText() {
                        Text = timetableObjects[0].LessonTag
                    },
                    new TileText() {
                        Text = timetableObjects[0].Rooms[0]
                    },
                    new TileText() {
                        Text = DateTime.Now.ToString("HH:mm")
                    }
                }
            };

            TileContent content = new TileContent() {
                Visual = new TileVisual() {
                    TileMedium = new TileBinding() {
                        Branding = TileBranding.Name,
                        Content = bindingContent
                    },
                    TileLarge = new TileBinding() {
                        Branding = TileBranding.Name,
                        Content = bindingContent
                    },
                    TileWide = new TileBinding() {
                        Branding = TileBranding.Name,
                        Content = bindingContent
                    }
                }
            };

            XmlDocument doc = content.GetXml();
            return doc;
        }

        private async Task<List<TimetableObject>> LoadTimetableBackup() {
            var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);
            if (readStream == null) {
                return new List<TimetableObject>();
            }
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<TimetableObject>));
            try {
                var timetableObjects = (List<TimetableObject>)serializer.ReadObject(readStream);
                return timetableObjects;
            }
            catch (Exception e) {
                Debug.Write(e.ToString());
                return new List<TimetableObject>();
            }
        }

        private static void UpdateTile(XmlDocument tileXml) {
            TileNotification tile = new TileNotification(tileXml);
            TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Update(tile);
        }
    }
}
