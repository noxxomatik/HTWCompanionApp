using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace HTWAppObjects
{
    public class CanteenModel
    {
        static CanteenModel instance = null;
        private const string feedTodayUri = "http://www.studentenwerk-dresden.de/feeds/speiseplan.rss?mid=9";
        private const string feedTomorrowUri = "http://www.studentenwerk-dresden.de/feeds/speiseplan.rss?mid=9&tag=morgen";

        private CanteenModel() { }

        public static CanteenModel GetInstance()
        {
            if (instance == null)
                instance = new CanteenModel();
            return instance;
        }

        public async Task<List<CanteenObject>> GetCanteenToday()
        {
            return await GetCanteen(feedTodayUri);
        }

        public async Task<List<CanteenObject>> GetCanteenTomorrow()
        {
            return await GetCanteen(feedTomorrowUri);
        }

        public async Task<List<CanteenObject>> GetCanteen(string canteenUri)
        {
            try {
                HttpClient client = new HttpClient();
                Stream stream = await client.GetStreamAsync(canteenUri);
                XDocument feedXML = XDocument.Load(stream);
                List<CanteenObject> foodList = new List<CanteenObject>();
                foreach (var item in feedXML.Descendants("item")) {
                    CanteenObject food = new CanteenObject();
                    food.Title = item.Element("title").Value;
                    food.Link = item.Element("link").Value;
                    food.Description = item.Element("description").Value;
                    food.Guid = item.Element("guid").Value;
                    food.Author = item.Element("author").Value;
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
}
