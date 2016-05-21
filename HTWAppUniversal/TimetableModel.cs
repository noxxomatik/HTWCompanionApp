using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Storage;

namespace HTWAppUniversal {
    class TimetableModel {
        private const string filename = "timetable.xml";

        /*
         * Get the timetable asynchronous from the server.
         * Returns an empty list of objects if anything fails.
         */
        public async Task<List<TimetableObject>> getTimetable(string stgJhr, string stg, string stgGrp) {
            // TODO: Regex zum Prüfen der Werte
            if (!stgJhr.Equals("") && !stg.Equals("") && !stgGrp.Equals("")) {
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
                    // backup timetable if connection fails the next time
                    await saveTimetableBackup(timetableObjects);

                    return timetableObjects;
                }
                catch (Exception e) {
                    Debug.WriteLine(e.ToString());
                    return await loadTimetableBackup();
                }                
            }
            else {
                return new List<TimetableObject>();
            }            
        }

        private async Task<bool> saveTimetableBackup(List<TimetableObject> timetableObjects) {
            try {
                StorageFile saveFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                using (Stream writeStream = await saveFile.OpenStreamForWriteAsync()) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<TimetableObject>));
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

        public async Task<List<TimetableObject>> loadTimetableBackup () {
            var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename);
            if (readStream == null) {
                return new List<TimetableObject>();
            }
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<TimetableObject>));
            var timetableObjects = (List<TimetableObject>)serializer.ReadObject(readStream);
            return timetableObjects;
        }
    }

    /*
    * One lesson in the timetable
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
    public class TimetableObject {
        public string lessonTag { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int week { get; set; }
        public int day { get; set; }
        public string beginTime { get; set; }
        public string endTime { get; set; }
        public string professor { get; set; }
        public string weeksOnly { get; set; }
        public List<string> rooms { get; set; }
    }
}
