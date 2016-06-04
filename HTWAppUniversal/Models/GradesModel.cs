using HTWAppObjects;
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
using System.Threading.Tasks;
using Windows.Storage;

namespace HTWAppUniversal {
    class GradesModel {
        static GradesModel instance = null;
        private const string filename = "grades";

        private GradesModel() {}

        public static GradesModel getInstance() {
            if (instance == null)
                instance = new GradesModel();
            return instance;
        }

        public async Task<List<GradeObject>> getGrades(string sNummer, string rZLogin) {
            // get additional information first
            List<CourseObject> courseObjects = await getCourses(sNummer, rZLogin);
            CourseObject course = courseObjects[0];
            // get the grades
            List<GradeObject> gradeObjects = await getGradesRemote(sNummer, rZLogin, course.AbschlNr, course.StgNr, course.POVersion);
            // backup grades
            if (gradeObjects.Count > 0) {
                await saveGradesBackup(gradeObjects, sNummer);
            }
            else {

            }
            return gradeObjects;
        }

        private async Task<List<CourseObject>> getCourses(string sNummer, string rZLogin) {
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

        private async Task<List<GradeObject>> getGradesRemote(string sNummer, string rZLogin, string abschlNr, string stgNr, string pOVersion) {
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
                    return gradeObjects;
                }
                catch (Exception e) {
                    Debug.WriteLine(e.ToString());
                    return new List<GradeObject>();
                }
            }
            else {
                return new List<GradeObject>();
            }
        }

        private async Task<bool> saveGradesBackup(List<GradeObject> gradeObjects, string sNummer) {
            try {
                StorageFile saveFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename + sNummer + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream writeStream = await saveFile.OpenStreamForWriteAsync()) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<GradeObject>));
                    serializer.WriteObject(writeStream, gradeObjects);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();
                }
                return true;
            }
            catch (Exception e) {
                throw new Exception("Unable to save grades", e);
            }
        }

        public async Task<List<GradeObject>> loadGradesBackup(string sNummer) {
            var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename + sNummer + ".xml");
            if (readStream == null) {
                return new List<GradeObject>();
            }
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<GradeObject>));
            var gradeObjects = (List<GradeObject>)serializer.ReadObject(readStream);
            return gradeObjects;
        }
    }
}
