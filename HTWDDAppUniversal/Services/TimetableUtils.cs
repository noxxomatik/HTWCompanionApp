﻿using System;
using HTWAppObjects;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Controls;

namespace HTWDDAppUniversal
{
    class TimetableUtils
    {
        public const int totalNumberofLessons = 8;
        public const String STACKPANEL = "stackpanel";

        private SettingsModel settingsModel;

        /*dictionary to map dates to appropriate number of row in the grid*/
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

        public TimetableUtils()
        {
            settingsModel = SettingsModel.getInstance();

            /*setup dictionary*/
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


        /*estimate if particular cell of grid hosts already a subelement
         * if yes: return subelement
         * if no: return null
         */
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


        /*merge element 1 und element2 in one stackpanel*/
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


        /*return if current week is even or odd
         * 0 = even
         * 1 = odd
         */
        public int isCurrentWeekEvenOrOdd()
        {
            DateTime today = DateTime.Now;
            DateTimeFormatInfo dtfi = DateTimeFormatInfo.CurrentInfo;
            Calendar cal = dtfi.Calendar;
            int weekNum = cal.GetWeekOfYear(today, dtfi.CalendarWeekRule, dtfi.FirstDayOfWeek);

            return weekNum % 2;
        }


        /*setup a textBlock with all required information for roomtimetable*/
        public TextBlock setupRoomTimetableTextBlock(TimetableObject item)
        {
            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            tb.MaxLines = 10;

            Thickness thickness = new Thickness();
            thickness.Right = thickness.Left = thickness.Top = thickness.Bottom = 1;
            tb.Padding = thickness;

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

        /*setup a textBlock with all required information for timetable*/
        public TextBlock setupTimetableTextBlock(TimetableObject item)
        {
            TextBlock tb = new TextBlock();
            tb.TextWrapping = TextWrapping.Wrap;
            tb.FontSize = 12;
            tb.MaxLines = 10;
            Thickness thickness = new Thickness();
            thickness.Right = thickness.Left = thickness.Top = thickness.Bottom = 1;
            tb.Padding = thickness;

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


        /*return Grid.Row of giben object*/
        public int getRowForTable(TimetableObject item)
        {
            int row;
            if (timeToRowDictionary.TryGetValue(item.BeginTime, out row))
                return row;
            else return -1;
        }


        /*check if spelling is correct: 
         * 1 letter (capital or lower)
         * undefined number of whitespaces
         * 3 digits
         * 
         * return null if not
         * return corrected string if there where no space or too many (white)spaces
         * */
        public String checkRoomSpell(String room) {
            String pattern = @"\b[A-Za-z]{1}\s*[0-9]{3}\b";
            Regex regex = new Regex(pattern);

            Match roomMatch = regex.Match(room);

            if (!roomMatch.Success)
            {
                return null;
            }

            if (roomMatch.Length == 4) //missing whitespace
            {
                room = room.Insert(1, " ");
            }

            if (roomMatch.Length > 5) //too many whitespaces or no space
            {
                Regex r = new Regex("\\s*");
                Match m = r.Match(room);
                room.Remove(1, m.Length);
                room = room.Insert(1, " ");
            }

            return room;
        }


        /*check if any of the given room-numbers suits input*/
        public bool lookupRoom(string room)
        {
            room = room.ToLower();
            List<String> all = settingsModel.Rooms;

            foreach(string listItem in all)
            {
                string lower = listItem.ToLower();

                Regex regex = new Regex(@"\b" + room);

                if (regex.Match(lower).Success)
                {
                    return true;
                }
            }

            return false;
        }
                
    }
}
