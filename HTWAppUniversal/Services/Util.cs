using System;
using HTWAppObjects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Controls;

namespace HTWAppUniversal
{
    class Util
    {
        public const int totalNumberofLessons = 8;
        public const String STACKPANEL = "stackpanel";

        private Dictionary<String, int> timeToRowDictionary;

        public Dictionary<String, int> TimeToRowDictionary
        {
            get
            {
                return timeToRowDictionary;
            }

            set
            {
                timeToRowDictionary = value;
            }
        }

        public Util()
        {
            timeToRowDictionary = new Dictionary<String, int>();
            timeToRowDictionary.Add("07:30:00", 0);
            timeToRowDictionary.Add("09:20:00", 1);
            timeToRowDictionary.Add("11:10:00", 2);
            timeToRowDictionary.Add("13:20:00", 3);
            timeToRowDictionary.Add("15:10:00", 4);
            timeToRowDictionary.Add("17:00:00", 5);
            timeToRowDictionary.Add("18:40:00", 6);
            timeToRowDictionary.Add("20:20:00", 7);
        }

        public FrameworkElement getChildOfGrid(Grid grid, int row, int col)
        {
            int colNum, rowNum;

            List<UIElement> elements = grid.Children.ToList<UIElement>();
            foreach(UIElement element in elements)
            {
                colNum = Grid.GetColumn((FrameworkElement)element);
                if(colNum == col)
                {
                    rowNum = Grid.GetRow((FrameworkElement)element);
                    if (rowNum == row)
                        return (FrameworkElement)element;
                }
            }
            return null;
        }

        public StackPanel createStackPanel(Grid grid, FrameworkElement element1, FrameworkElement element2)
        {
            StackPanel stackpanel = new StackPanel();
            stackpanel.Name = STACKPANEL;
            stackpanel.Orientation = Orientation.Vertical;
            Grid.SetColumn(stackpanel, Grid.GetColumn(element1));

            if(null != element1.Parent)
                grid.Children.Remove((UIElement) element1);
            if(null != element2.Parent)
                grid.Children.Remove((UIElement) element2);

            stackpanel.Children.Add(element1);
            stackpanel.Children.Add(element2);

            return stackpanel;
        }

        public int isCurrentWeekEvenOrOdd()
        {
            DateTime today = new DateTime();
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dtfi.Calendar;
            int weekNum = cal.GetWeekOfYear(today, dtfi.CalendarWeekRule, dtfi.FirstDayOfWeek);

            return weekNum % 2;
        }

        public TextBlock setupRoomTimetableTextBlock(TimetableObject item)
        {
            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            tb.MaxLines = 10;

            Span lessonSpan = new Span();
            Run lesson = new Run();
            lesson.Text = item.LessonTag;
            lessonSpan.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
            lessonSpan.Inlines.Add(lesson);

            Span profSpan = new Span();
            Run prof = new Run();
            prof.Text = item.Professor;
            profSpan.FontStyle = Windows.UI.Text.FontStyle.Normal;
            profSpan.Inlines.Add(prof);

            tb.Inlines.Add(lessonSpan);
            tb.Inlines.Add(new LineBreak());
            tb.Inlines.Add(profSpan);

            Grid.SetColumn(tb, item.Day);
            return tb;
        }

        public TextBlock setupTimetableTextBlock(TimetableObject item)
        {
            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            tb.MaxLines = 10;

            Span lessonSpan = new Span();
            Run lesson = new Run();
            lesson.Text = item.LessonTag + " (" + item.Type + ")";
            lessonSpan.FontWeight = Windows.UI.Text.FontWeights.SemiBold;
            lessonSpan.Inlines.Add(lesson);

            Span profSpan = new Span();
            Run prof = new Run();
            prof.Text = item.Professor.Equals("")? "verschiedene Dozenten" : item.Professor ;
            profSpan.FontStyle = Windows.UI.Text.FontStyle.Italic;
            profSpan.Inlines.Add(prof);

            Span roomSpan = new Span();
            Run roomRun = new Run();
            foreach (String room in item.Rooms)
            {
                roomRun.Text += room + ' ';
            }
            roomSpan.FontStyle = Windows.UI.Text.FontStyle.Normal;
            roomSpan.Inlines.Add(roomRun);

            tb.Inlines.Add(lessonSpan);
            tb.Inlines.Add(new LineBreak());
            tb.Inlines.Add(profSpan);
            tb.Inlines.Add(new LineBreak());
            tb.Inlines.Add(roomSpan);

            Grid.SetColumn(tb, item.Day);

            return tb;
        }

        public int getRowForTable(TimetableObject item)
        {
            int row;
            if (timeToRowDictionary.TryGetValue(item.BeginTime, out row))
                return row;
            else return -1;
        }
    }
}
