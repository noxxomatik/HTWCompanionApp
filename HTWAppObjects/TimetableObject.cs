using NotificationsExtensions.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using Windows.Storage;
using Windows.UI.Notifications;

/*
    One lesson in the timetable
*/
/*
    lessonTag - Kürzel
    name - ganzer Name
    type - V Vorlesung Pr Praktikum
    week - 0 jede Woche 1 ungerade Woche 2 gerade Woche
    day - Wochentag beginnend mit 1
    beginTime - 11:10:00
    endTime - 12:40:00
    professor - Familienname
    WeeksOnly - Anzahl Wochen in denen Lehrveranstaltung stattfindet
    Rooms - Array mit Raumnummern ["S 128"]
*/
namespace HTWAppObjects
{
    public class TimetableObject
    {
        public string LessonTag { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int Week { get; set; }
        public int Day { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
        public string Professor { get; set; }
        public string WeeksOnly { get; set; }
        public List<string> Rooms { get; set; }

        private const string filename = "timetable.xml";

        public static async Task<XmlDocument> GetNextLessonXml() {
            // don´t download a new timetable, use the stored one
            List<TimetableObject> timetableObjects = await LoadTimetableBackup();

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

            foreach (TimetableObject timetableObject in timetableObjects) {
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
                    // add zo next week only
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
            }
            // if there is nothing in this week get next week
            int dayOfWeek = 0;
            while (nextLesson == null && dayOfWeek < 7) {
                nextLessonInt = 0;
                while (nextLesson == null && nextLessonInt < 8) {
                    nextLesson = nextWeekDictionary[dayOfWeek][nextLessonInt];
                    nextLessonInt++;
                }
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
                        Text = nextLesson.Rooms[0] + (nextLesson.Rooms.Count > 0 ? " u. a." : "")
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

        private static async Task<List<TimetableObject>> LoadTimetableBackup() {
            var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);
            if (readStream == null) {
                return new List<TimetableObject>();
            }
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<TimetableObject>));
            try {
                var timetableObjects = (List<TimetableObject>) serializer.ReadObject(readStream);
                return timetableObjects;
            }
            catch (Exception e) {
                Debug.Write(e.ToString());
                return new List<TimetableObject>();
            }
        }

        public static void UpdateTile(XmlDocument tileXml) {
            TileNotification tile = new TileNotification(tileXml);
            TileUpdater tileUpdater = TileUpdateManager.CreateTileUpdaterForApplication();
            tileUpdater.Update(tile);
        }
    }
}
