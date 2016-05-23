using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HTWAppUniversal {
    class CanteenModel {
        private const string feedTodayUri = "http://www.studentenwerk-dresden.de/feeds/speiseplan.rss?mid=9";
        private const string feedTomorrowUri = "http://www.studentenwerk-dresden.de/feeds/speiseplan.rss?mid=9&tag=morgen";

        public async Task<List<CanteenObject>> getCanteenToday() {
            return await getCanteen(feedTodayUri);
        }

        public async Task<List<CanteenObject>> getCanteenTomorrow() {
            return await getCanteen(feedTomorrowUri);
        }

        public async Task<List<CanteenObject>> getCanteen(string canteenUri) {
            try {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync(canteenUri);
                XDocument feedXML = XDocument.Load(stream);
                List<CanteenObject> foodList = new List<CanteenObject>();
                foreach (var item in feedXML.Descendants("item")) {
                    CanteenObject food = new CanteenObject();
                    food.title = item.Element("title").Value;
                    food.link = item.Element("link").Value;
                    food.description = item.Element("description").Value;
                    food.guid = item.Element("guid").Value;
                    food.author = item.Element("author").Value;
                    foodList.Add(food);
                }
                return foodList;
            }
            catch (Exception e) {
                Debug.WriteLine(e.ToString());
                return new List<CanteenObject>();
            }
        }
    }

    public class CanteenObject {
        public string title { get; set; }
        public string link { get; set; }
        public string description { get; set; }
        public string guid { get; set; }
        public string author { get; set; }
    }
}
