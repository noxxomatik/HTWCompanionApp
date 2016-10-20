using HTWAppObjects.Objects;
using Newtonsoft.Json;
using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

namespace HTWAppObjects
{
    public class TimetableModel {
        static TimetableModel instance = null;
        private const string filename = "timetable.xml";

        private TimetableModel () {}

        public static TimetableModel getInstance() {
            if (instance == null)
                instance = new TimetableModel();
            return instance;
        }

        /*
         * Get the timetable asynchronous from the server.
         * Returns an empty list of objects if anything fails.
         */
        public async Task<TimetableObjectsList> getTimetable(string stgJhr, string stg, string stgGrp)  {
            // TODO: Regex zum Prüfen der Werte
            if (stgJhr != null && stg != null && stgGrp != null && !stgJhr.Equals("") && !stg.Equals("") && !stgGrp.Equals("")) {
                try {
                    string requestData = WebUtility.UrlEncode("StgJhr") + "=" + WebUtility.UrlEncode(stgJhr) + "&"
                        + WebUtility.UrlEncode("Stg") + "=" + WebUtility.UrlEncode(stg) + "&"
                        + WebUtility.UrlEncode("StgGrp") + "=" + WebUtility.UrlEncode(stgGrp);

                    Uri uri = new Uri("https://www2.htw-dresden.de/~app/API/GetTimetable.php?" + requestData);
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(uri);
                    string content = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine(content);

                    List<TimetableObject> timetableObjects = JsonConvert.DeserializeObject<List<TimetableObject>>(content);
                    TimetableObjectsList timetableObjectsList = new TimetableObjectsList();
                    timetableObjectsList.timetableObjects = timetableObjects;
                    timetableObjectsList.timestamp = DateTime.Now;

                    // backup timetable if connection fails the next time
                    await saveTimetableBackup(timetableObjectsList);

                    return timetableObjectsList;
                }
                catch (Exception e) {
                    Debug.WriteLine(e.ToString());
                    return null;
                }                
            }
            else {
                return null;
            }            
        }

        private async Task<bool> saveTimetableBackup(TimetableObjectsList timetableObjects) {
            try {
                StorageFile saveFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                using (Stream writeStream = await saveFile.OpenStreamForWriteAsync()) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(TimetableObjectsList));
                    serializer.WriteObject(writeStream, timetableObjects);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();
                }
                return true;
            }
            catch (Exception e) {
                throw new Exception("Unable to save the timetable", e);
            }
        }

        public static async Task<XmlDocument> GetNextLessonXml() {
            // don´t download a new timetable, use the stored one
            TimetableObjectsList timetableObjects = await LoadTimetableBackup();
            if (timetableObjects != null) {

                // find the next lesson
                TimetableObject nextLesson = null;
                DateTime now = DateTime.Now;
                DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
                Calendar cal = dtfi.Calendar;
                int weekNum = cal.GetWeekOfYear(now, dtfi.CalendarWeekRule, dtfi.FirstDayOfWeek);
                int weekParity = (weekNum % 2) == 0 ? 2 : 1; // 1 odd, 2 even

                // create sorted list of lessons
                Dictionary<String, int> timeToIntDictionary = new Dictionary<String, int>();
                timeToIntDictionary.Add("07:30:00", 0);
                timeToIntDictionary.Add("09:20:00", 1);
                timeToIntDictionary.Add("11:10:00", 2);
                timeToIntDictionary.Add("13:20:00", 3);
                timeToIntDictionary.Add("15:10:00", 4);
                timeToIntDictionary.Add("17:00:00", 5);
                timeToIntDictionary.Add("18:40:00", 6);
                timeToIntDictionary.Add("20:20:00", 7);

                Dictionary<int, Dictionary<int, TimetableObject>> thisWeekDictionary = new Dictionary<int, Dictionary<int, TimetableObject>>();
                Dictionary<int, Dictionary<int, TimetableObject>> nextWeekDictionary = new Dictionary<int, Dictionary<int, TimetableObject>>();
                for (int i = 0; i < 7; i++) {
                    Dictionary<int, TimetableObject> dayDictionary = new Dictionary<int, TimetableObject>();
                    Dictionary<int, TimetableObject> dayDictionaryNext = new Dictionary<int, TimetableObject>();
                    for (int k = 0; k < 8; k++) {
                        dayDictionary.Add(k, null);
                        dayDictionaryNext.Add(k, null);
                    }
                    thisWeekDictionary.Add(i, dayDictionary);
                    nextWeekDictionary.Add(i, dayDictionaryNext);
                }

                foreach (TimetableObject timetableObject in timetableObjects.timetableObjects) {
                    if (timetableObject.Week == 0) {
                        //add to this and next week
                        thisWeekDictionary[timetableObject.Day][timeToIntDictionary[timetableObject.BeginTime]] = timetableObject;
                        nextWeekDictionary[timetableObject.Day][timeToIntDictionary[timetableObject.BeginTime]] = timetableObject;
                    }
                    else if (timetableObject.Week == weekParity) {
                        // add to this week only
                        thisWeekDictionary[timetableObject.Day][timeToIntDictionary[timetableObject.BeginTime]] = timetableObject;
                    }
                    else {
                        // add to next week only
                        nextWeekDictionary[timetableObject.Day][timeToIntDictionary[timetableObject.BeginTime]] = timetableObject;
                    }
                }

                // search through the list
                int nextLessonInt = 8; // 8 = after last lesson
                if (now < new DateTime(now.Year, now.Month, now.Day, 7, 30, 0, DateTimeKind.Local)) {
                    nextLessonInt = 0;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 9, 20, 0, DateTimeKind.Local)) {
                    nextLessonInt = 1;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 11, 10, 0, DateTimeKind.Local)) {
                    nextLessonInt = 2;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 13, 20, 0, DateTimeKind.Local)) {
                    nextLessonInt = 3;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 15, 10, 0, DateTimeKind.Local)) {
                    nextLessonInt = 4;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 17, 00, 0, DateTimeKind.Local)) {
                    nextLessonInt = 5;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 18, 40, 0, DateTimeKind.Local)) {
                    nextLessonInt = 6;
                }
                else if (now < new DateTime(now.Year, now.Month, now.Day, 20, 20, 0, DateTimeKind.Local)) {
                    nextLessonInt = 7;
                }
                // is there a lesson on this day
                while (nextLesson == null && nextLessonInt < 8) {
                    nextLesson = thisWeekDictionary[(int) now.DayOfWeek][nextLessonInt];
                    nextLessonInt++;
                }
                // if there is nothing on this day get next lesson in this week
                int j = 1;
                while (nextLesson == null && ((int) now.DayOfWeek) + j < 7) {
                    nextLessonInt = 0;
                    while (nextLesson == null && nextLessonInt < 8) {
                        nextLesson = thisWeekDictionary[(int) now.DayOfWeek + j][nextLessonInt];
                        nextLessonInt++;
                    }
                    j++;
                }
                // if there is nothing in this week get next week
                int dayOfWeek = 0;
                while (nextLesson == null && dayOfWeek < 7) {
                    nextLessonInt = 0;
                    while (nextLesson == null && nextLessonInt < 8) {
                        nextLesson = nextWeekDictionary[dayOfWeek][nextLessonInt];
                        nextLessonInt++;
                    }
                    dayOfWeek++;
                }
                // if it found nothing yet the timetable is totally empty...
                XmlDocument doc = null;
                if (nextLesson != null) {
                    // build the tile
                    TileBindingContentAdaptive bindingContent = new TileBindingContentAdaptive() {
                        Children = {
                    new TileText() {
                        Text = "Nächste Stunde",
                        Style = TileTextStyle.CaptionSubtle
                    },
                    new TileText() {
                        Text = nextLesson.LessonTag + " (" + nextLesson.Type + ")"
                    },
                    new TileText() {
                        Text = nextLesson.Rooms[0] + (nextLesson.Rooms.Count > 1 ? " u. a." : "")
                    },
                    new TileText() {
                        Text = nextLesson.BeginTime.Remove(5, 3) + " Uhr"
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

                    doc = content.GetXml();
                }
                return doc;
            }
            return null;
        }

        public static async Task<TimetableObjectsList> LoadTimetableBackup() {
            try {
                var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);
                if (readStream == null) {
                    return null;
                }
                DataContractSerializer serializer = new DataContractSerializer(typeof(TimetableObjectsList));
                try {
                    var timetableObjects = (TimetableObjectsList) serializer.ReadObject(readStream);
                    return timetableObjects;
                }
                catch (Exception e) {
                    Debug.Write(e.ToString());
                    return null;
                }
            }
            catch {
                return null;
            }
            
        }

        public static void UpdateTile(XmlDocument tileXml) {
            TileNotification tile = new TileNotification(tileXml);
            TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Update(tile);
        }
    }
}
