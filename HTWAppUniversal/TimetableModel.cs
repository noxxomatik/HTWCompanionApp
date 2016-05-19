using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HTWAppUniversal {
    class TimetableModel {
        public async Task<string> getTimetable(string stgJhr, string stg, string stgGrp) {

            string requestData = WebUtility.UrlEncode("StgJhr") + "=" + WebUtility.UrlEncode(stgJhr) + "&"
                + WebUtility.UrlEncode("Stg") + "=" + WebUtility.UrlEncode(stg) + "&"
                + WebUtility.UrlEncode("StgGrp") + "=" + WebUtility.UrlEncode(stgGrp);

            Uri uri = new Uri("https://www2.htw-dresden.de/~app/API/GetTimetable.php?" + requestData);
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);
            string content = await response.Content.ReadAsStringAsync();

            Debug.WriteLine(content);

            return content;
        }
    }
}
