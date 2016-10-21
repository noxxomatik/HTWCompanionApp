using System.Collections.Generic;
using System.Linq;
using Windows.Storage;

namespace HTWAppObjects
{
    // use settings as roaming data to sync them with other systems
    // https://msdn.microsoft.com/en-us/windows/uwp/app-settings/store-and-retrieve-app-data
    public class SettingsModel
    {
        static SettingsModel instance = null;
        ApplicationDataContainer roamingSettings;
        StorageFolder roamingFolder;

        // settings
        string sNummer;
        string rZLogin;
        string stgJhr;
        string stg;
        string stgGrp;
        List<string> rooms;

        // Singleton pattern
        private SettingsModel()
        {
            roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingFolder = ApplicationData.Current.RoamingFolder;
        }

        public static SettingsModel GetInstance()
        {
            if (instance == null)
                instance = new SettingsModel();
            return instance;
        }

        /// <summary>
        /// Bibliotheksnummer (inkl. fuehrendem s)
        /// </summary>
        public string SNummer
        {
            get {
                sNummer = (string) roamingSettings.Values["sNummer"];
                return sNummer;
            }
            set {
                sNummer = value;
                roamingSettings.Values["sNummer"] = sNummer;
            }
        }

        /// <summary>
        /// Passwort
        /// </summary>
        public string RZLogin
        {
            get {
                rZLogin = (string) roamingSettings.Values["RZLogin"];
                return rZLogin;
            }
            set {
                rZLogin = value;
                roamingSettings.Values["RZLogin"] = rZLogin;
            }
        }

        /// <summary>
        /// Studienjahrgang (z.B. 15)
        /// </summary>
        public string StgJhr
        {
            get {
                stgJhr = (string) roamingSettings.Values["StgJhr"];
                return stgJhr;
            }
            set {
                stgJhr = value;
                roamingSettings.Values["StgJhr"] = stgJhr;
            }
        }

        /// <summary>
        /// Studiengang (z.B. 044)
        /// </summary>
        public string Stg
        {
            get {
                stg = (string) roamingSettings.Values["Stg"];
                return stg;
            }
            set {
                stg = value;
                roamingSettings.Values["Stg"] = stg;
            }
        }

        /// <summary>
        /// Studiengruppe (z.B. 73-CM)
        /// </summary>
        public string StgGrp
        {
            get {
                stgGrp = (string) roamingSettings.Values["StgGrp"];
                return stgGrp;
            }
            set {
                stgGrp = value;
                roamingSettings.Values["StgGrp"] = stgGrp;
            }
        }

        /// <summary>
        /// anzuzeigende Raeume fuer Raumbelegung
        /// </summary>
        public List<string> Rooms
        {
            get {
                rooms = roamingSettings.Values["Rooms"] != null ? ((string[]) roamingSettings.Values["Rooms"]).ToList<string>() : new List<string>();
                return rooms;
            }
            set {
                rooms = value;
                roamingSettings.Values["Rooms"] = rooms.Count > 0 ? rooms.ToArray() : null;
            }
        }
    }
}
