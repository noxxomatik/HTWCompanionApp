using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace HTWAppUniversal {
    // use settings as roaming data to sync them with other systems
    // https://msdn.microsoft.com/en-us/windows/uwp/app-settings/store-and-retrieve-app-data
    class SettingsModel {
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
        private SettingsModel () {
            roamingSettings = ApplicationData.Current.RoamingSettings;
            roamingFolder = ApplicationData.Current.RoamingFolder;
        }

        public static SettingsModel getInstance () {
            if (instance == null)
                instance = new SettingsModel();
            return instance;
        }

        public string SNummer {
            get
            {
                sNummer = (string) roamingSettings.Values["sNummer"];
                return sNummer;
            }

            set
            {
                sNummer = value;
                roamingSettings.Values["sNummer"] = sNummer;                
            }
        }

        public string RZLogin
        {
            get
            {
                rZLogin = (string)roamingSettings.Values["RZLogin"];
                return rZLogin;
            }

            set
            {
                rZLogin = value;
                roamingSettings.Values["RZLogin"] = rZLogin;
            }
        }

        public string StgJhr
        {
            get
            {
                stgJhr = (string)roamingSettings.Values["StgJhr"];
                return stgJhr;
            }

            set
            {
                stgJhr = value;
                roamingSettings.Values["StgJhr"] = stgJhr;
            }
        }

        public string Stg
        {
            get
            {
                stg = (string)roamingSettings.Values["Stg"];
                return stg;
            }

            set
            {
                stg = value;
                roamingSettings.Values["Stg"] = stg;
            }
        }

        public string StgGrp
        {
            get
            {
                stgGrp = (string)roamingSettings.Values["StgGrp"];
                return stgGrp;
            }

            set
            {
                stgGrp = value;
                roamingSettings.Values["StgGrp"] = stgGrp;
            }
        }

        public List<string> Rooms
        {
            get
            {
                rooms = ((string[]) roamingSettings.Values["Rooms"]).ToList<string>();
                return rooms;
            }

            set
            {
                rooms = value;
                roamingSettings.Values["Rooms"] = rooms.ToArray();
            }
        }
    }
}
