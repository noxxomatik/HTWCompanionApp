﻿using Newtonsoft.Json;
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
    public class RoomTimetableModel
    {
        static RoomTimetableModel instance = null;
        private const string filename = "roomTimetable";

        private RoomTimetableModel() { }

        public static RoomTimetableModel GetInstance()
        {
            if (instance == null)
                instance = new RoomTimetableModel();
            return instance;
        }

        /*
         * Get the timetable for the room asynchronous from the server.
         * Room must be from pattern [
         * Returns an empty list of objects if anything fails.
         */
        public async Task<List<TimetableObject>> GetRoomTimetable(string room)
        {
            // TODO: Regex zum Prüfen der Werte
            if (room != null && !room.Equals("")) {
                room = room.ToLower();
                try {
                    string requestData = WebUtility.UrlEncode("Room") + "=" + WebUtility.UrlEncode(room);

                    Uri uri = new Uri("https://www2.htw-dresden.de/~app/API/GetTimetable.php?" + requestData);
                    HttpClient client = new HttpClient();
                    HttpResponseMessage response = await client.GetAsync(uri);
                    string content = await response.Content.ReadAsStringAsync();

                    Debug.WriteLine(content);

                    List<TimetableObject> timetableObjects = JsonConvert.DeserializeObject<List<TimetableObject>>(content);
                    // backup timetable if connection fails the next time
                    await SaveRoomTimetableBackup(timetableObjects, room);

                    return timetableObjects;
                }
                catch (Exception e) {
                    Debug.WriteLine(e.ToString());
                    return await LoadRoomTimetableBackup(room);
                }
            }
            else {
                return new List<TimetableObject>();
            }
        }

        private async Task<bool> SaveRoomTimetableBackup(List<TimetableObject> timetableObjects, string room)
        {
            room = room.ToLower();
            try {
                StorageFile saveFile = await ApplicationData.Current.LocalFolder.CreateFileAsync(filename + room + ".xml", CreationCollisionOption.ReplaceExisting);
                using (Stream writeStream = await saveFile.OpenStreamForWriteAsync()) {
                    DataContractSerializer serializer = new DataContractSerializer(typeof(List<TimetableObject>));
                    serializer.WriteObject(writeStream, timetableObjects);
                    await writeStream.FlushAsync();
                    writeStream.Dispose();
                }
                return true;
            }
            catch (Exception e) {
                throw new Exception("Unable to save the room timetable", e);
            }
        }

        public async Task<List<TimetableObject>> LoadRoomTimetableBackup(string room)
        {
            room = room.ToLower();
            var readStream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync(filename + room + ".xml");
            if (readStream == null) {
                return new List<TimetableObject>();
            }
            DataContractSerializer serializer = new DataContractSerializer(typeof(List<TimetableObject>));
            var timetableObjects = (List<TimetableObject>) serializer.ReadObject(readStream);
            return timetableObjects;
        }
    }
}

