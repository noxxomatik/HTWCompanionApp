using HTWAppObjects.Objects;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Windows.Storage;

namespace HTWAppObjects
{
    public class GradesModel
    {
        static GradesModel instance = null;
        private const string filename = "grades";

        private GradesModel() { }

        public static GradesModel GetInstance()
        {
            if (instance == null)
                instance = new GradesModel();
            return instance;
        }

        public async Task<GradeObjectsList> GetGrades(string sNummer, string rZLogin)
        {
            GradeObjectsList gradeObjectsList;
            try {
                // get additional information first
                List<CourseObject> courseObjects = await GetCourses(sNummer, rZLogin);
                CourseObject course = courseObjects[0];
                // get the grades
                gradeObjectsList = await GetGradesRemote(sNummer, rZLogin, course.AbschlNr, course.StgNr, course.POVersion);
                // backup grades
                if (gradeObjectsList.gradeObjects.Count > 0) {
                    await SaveGradesBackup(gradeObjectsList, sNummer);
                }
            }
            catch {
                gradeObjectsList = await LoadGradesBackup(sNummer);
            }
            return gradeObjectsList;
        }

        /*
         * Returns the number of new grades.
         */
        public async Task<int> GetNewGradesCount(string sNummer, string rZLogin)
        {
            GradeObjectsList backupGrades = await LoadGradesBackup(sNummer);
            GradeObjectsList newGrades = await GetGrades(sNummer, rZLogin);
            if (backupGrades != null && newGrades != null) {
                int count = newGrades.gradeObjects.Count - backupGrades.gradeObjects.Count;
                return count >= 0 ? count : -1;
            }
            else
                return -1;
        }

        private async Task<List<CourseObject>> GetCourses(string sNummer, string rZLogin)
        {
            // TODO: Regex zum Prüfen der Werte
            if (!sNummer.Equals("") && !rZLogin.Equals("")) {
                try {
                    string requestData = WebUtility.UrlEncode("sNummer") + "=" + WebUtility.UrlEncode(sNummer) + "&"
                        + WebUtility.UrlEncode("RZLogin") + "=" + WebUtility.UrlEncode(rZLogin);
                    Uri uri = new Uri("https://wwwqis.htw-dresden.de/appservice/getcourses?" + requestData);
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(uri, new StringContent(""));
                    string content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    List<CourseObject> courseObjects = JsonConvert.DeserializeObject<List<CourseObject>>(content);
                    return courseObjects;
                }
                catch (Exception e) {
                    Debug.WriteLine(e.ToString());
                    return new List<CourseObject>();
                }
            }
            else {
                return new List<CourseObject>();
            }
        }

        private async Task<GradeObjectsList> GetGradesRemote(string sNummer, string rZLogin, string abschlNr, string stgNr, string pOVersion)
        {
            // TODO: Regex zum Prüfen der Werte
            if (!sNummer.Equals("") && !rZLogin.Equals("") && !abschlNr.Equals("") && !stgNr.Equals("") && !pOVersion.Equals("")) {
                try {
                    string requestData = WebUtility.UrlEncode("sNummer") + "=" + WebUtility.UrlEncode(sNummer) + "&"
                        + WebUtility.UrlEncode("RZLogin") + "=" + WebUtility.UrlEncode(rZLogin) + "&"
                        + WebUtility.UrlEncode("AbschlNr") + "=" + WebUtility.UrlEncode(abschlNr) + "&"
                        + WebUtility.UrlEncode("StgNr") + "=" + WebUtility.UrlEncode(stgNr) + "&"
                        + WebUtility.UrlEncode("POVersion") + "=" + WebUtility.UrlEncode(pOVersion);
                    Uri uri = new Uri("https://wwwqis.htw-dresden.de/appservice/getgrades?" + requestData);
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.PostAsync(uri, new StringContent(""));
                    string content = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine(content);
                    List<GradeObject> gradeObjects = JsonConvert.DeserializeObject<List<GradeObject>>(content);
                    GradeObjectsList gradeObjectsList = new GradeObjectsList();
                    gradeObjectsList.gradeObjects = gradeObjects;
                    gradeObjectsList.timestamp = DateTime.Now;
                    return gradeObjectsList;
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

        private async Task<bool> SaveGradesBackup(GradeObjectsList gradeObjectsList, string sNummer)
        {
            try {
                StorageFile saveFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename + sNummer + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream writeStream = await saveFile.OpenStreamForWriteAsync()) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(GradeObjectsList));
                    serializer.WriteObject(writeStream, gradeObjectsList);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();
                }
                return true;
            }
            catch (Exception e) {
                throw new Exception("Unable to save grades", e);
            }
        }

        public async Task<GradeObjectsList> LoadGradesBackup(string sNummer)
        {
            try {
                var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename + sNummer + ".xml");
                if (readStream == null) {
                    return null;
                }
                DataContractSerializer serializer = new DataContractSerializer(typeof(GradeObjectsList));
                var gradeObjectsList = (GradeObjectsList) serializer.ReadObject(readStream);
                return gradeObjectsList;
            }
            catch {
                return null;
            }
        }
    }
}
