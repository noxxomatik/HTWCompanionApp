using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HTWAppObjects;
using System.Collections.Generic;
using Windows.Data.Xml.Dom;
using System.Diagnostics;
using System.Threading.Tasks;
using HTWAppObjects.Objects;

namespace HTWDDAppUniversal.Views
{
    public sealed partial class TimetablePage : Page
    {
        private TimetableModel timetableModel;
        private List<TimetableObject> lessons;
        private TimetableUtils timetableUtils;
        private SettingsModel settingsModel;

        public TimetablePage()
        {
            InitializeComponent();
            //NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //timetableUtils = new TimetableUtils();
            LoadTimetablesBackup();
        }

        async Task LoadTimetables()
        {
            timetableModel = TimetableModel.GetInstance();
            timetableUtils = new TimetableUtils();
            settingsModel = SettingsModel.GetInstance();
            TimetableObjectsList timetableObjects = await timetableModel.GetTimetable(settingsModel.StgJhr, settingsModel.Stg, settingsModel.StgGrp);
            if (timetableObjects != null) {
                // clear the timetables
                timetableUtils.clearTimetable(timetableThisWeek);
                timetableUtils.clearTimetable(timetableNextWeek);

                Lessons = timetableObjects.timetableObjects;

                UpdateLiveTile();

                /*find out if current week is even or odd*/
                int evenOdd = timetableUtils.isCurrentWeekEvenOrOdd();

                foreach (TimetableObject timetableObject in Lessons) {
                    int[] row = timetableUtils.getRowForTable(timetableObject);
                    if (row[0] != -1 && row[1] != -1) {
                        for (int currentRow = row[0]; currentRow <= row[1]; currentRow++) {
                            // prepare the button for this lesson
                            TimetableItem timetableItem = new TimetableItem();
                            string room = "";
                            if (timetableObject.Rooms.Count > 1) {
                                room = timetableObject.Rooms[0] + " ...";
                            }
                            else {
                                room = timetableObject.Rooms[0];
                            }
                            timetableItem.TextBlock.Text = timetableObject.LessonTag + "\n"
                                + timetableObject.Type + "\n"
                                + room;
                            Grid.SetColumn(timetableItem, timetableObject.Day);
                            Grid.SetRow(timetableItem, currentRow);
                            // switch for the week number
                            switch (timetableObject.Week) {
                                /*lesson takes place every week*/
                                case 0: {
                                        timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                        // clone the button
                                        TimetableItem clone = new TimetableItem();
                                        Grid.SetColumn(clone, timetableObject.Day);
                                        Grid.SetRow(clone, currentRow);
                                        clone.TextBlock.Text = timetableItem.TextBlock.Text;
                                        timetableUtils.addLessonToWeek(timetableNextWeek, currentRow, clone);
                                        break;
                                    }
                                /*only at odd weeks*/
                                case 1: {
                                        /*if current week is even add it to next week*/
                                        if (evenOdd == 0) {
                                            timetableUtils.addLessonToWeek(timetableNextWeek, currentRow, timetableItem);
                                        }
                                        else {
                                            timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                        }
                                        break;
                                    }
                                /*only at even weeks*/
                                case 2: {
                                        /*if current week is even add it to this week*/
                                        if (evenOdd == 0) {
                                            timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                        }
                                        else {
                                            timetableUtils.addLessonToWeek(timetableNextWeek, currentRow, timetableItem);
                                        }
                                        break;
                                    }
                                default: {
                                        break;
                                    }
                            }
                        }
                    }
                }
                // update timestamp
                timestampThisWeek.Text = timestampNextWeek.Text = "Stand: " + timetableObjects.timestamp.ToLocalTime().ToString();
            }
        }

        async void LoadTimetablesBackup()
        {
            timetableModel = TimetableModel.GetInstance();
            timetableUtils = new TimetableUtils();
            settingsModel = SettingsModel.GetInstance();
            TimetableObjectsList timetableObjects = await TimetableModel.LoadTimetableBackup();
            if (timetableObjects != null) {
                // clear the timetables
                timetableUtils.clearTimetable(timetableThisWeek);
                timetableUtils.clearTimetable(timetableNextWeek);

                Lessons = timetableObjects.timetableObjects;

                UpdateLiveTile();

                /*find out if current week is even or odd*/
                int evenOdd = timetableUtils.isCurrentWeekEvenOrOdd();

                foreach (TimetableObject timetableObject in Lessons) {
                    int[] row = timetableUtils.getRowForTable(timetableObject);
                    if (row[0] != -1 && row[1] != -1) {
                        for (int currentRow = row[0]; currentRow <= row[1]; currentRow++) {
                            // prepare the button for this lesson
                            TimetableItem timetableItem = new TimetableItem();
                            string room = "";
                            if (timetableObject.Rooms.Count > 1) {
                                room = timetableObject.Rooms[0] + " ...";
                            }
                            else {
                                room = timetableObject.Rooms[0];
                            }
                            timetableItem.TextBlock.Text = timetableObject.LessonTag + "\n"
                                + timetableObject.Type + "\n"
                                + room;
                            Grid.SetColumn(timetableItem, timetableObject.Day);
                            Grid.SetRow(timetableItem, currentRow);
                            // switch for the week number
                            switch (timetableObject.Week) {
                                /*lesson takes place every week*/
                                case 0: {
                                        timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                        // clone the button
                                        TimetableItem clone = new TimetableItem();
                                        Grid.SetColumn(clone, timetableObject.Day);
                                        Grid.SetRow(clone, currentRow);
                                        clone.TextBlock.Text = timetableItem.TextBlock.Text;
                                        timetableUtils.addLessonToWeek(timetableNextWeek, currentRow, clone);
                                        break;
                                    }
                                /*only at odd weeks*/
                                case 1: {
                                        /*if current week is even add it to next week*/
                                        if (evenOdd == 0) {
                                            timetableUtils.addLessonToWeek(timetableNextWeek, currentRow, timetableItem);
                                        }
                                        else {
                                            timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                        }
                                        break;
                                    }
                                /*only at even weeks*/
                                case 2: {
                                        /*if current week is even add it to this week*/
                                        if (evenOdd == 0) {
                                            timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                        }
                                        else {
                                            timetableUtils.addLessonToWeek(timetableNextWeek, currentRow, timetableItem);
                                        }
                                        break;
                                    }
                                default: {
                                        break;
                                    }
                            }
                        }
                    }
                }
                // update timestamp
                timestampThisWeek.Text = timestampNextWeek.Text = "Stand: " + timetableObjects.timestamp.ToLocalTime().ToString();
            }
        }

        // update the live tile
        private static async Task UpdateLiveTile()
        {
            try {
                // Load the item.
                XmlDocument tileXml = await TimetableModel.GetNextLessonXml();
                if (tileXml == null)
                    throw new Exception("Timetable is empty.");
                // Update the live tile with the item.
                TimetableModel.UpdateTile(tileXml);
            }
            catch {
                Debug.WriteLine("Missing timteable item. First load the timetable.");
            }
        }

        public List<TimetableObject> Lessons
        {
            get {
                return lessons;
            }

            set {
                lessons = value;
            }
        }

        private async void SymbolIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            symbolIcon.Visibility = Visibility.Collapsed;
            progressRing.Visibility = Visibility.Visible;

            await LoadTimetables();

            progressRing.Visibility = Visibility.Collapsed;
            symbolIcon.Visibility = Visibility.Visible;
        }
    }
}
