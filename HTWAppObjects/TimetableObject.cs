using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    public class TimetableObject {
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
    }
}
