using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using HTWAppObjects;
using System.Collections.Generic;
using Windows.Data.Xml.Dom;
using System.Diagnostics;
using System.Threading.Tasks;

namespace HTWDDAppUniversal.Views
{
    public sealed partial class TimetablePage : Page
    {
        private TimetableModel timetableModel;
        private List<TimetableObject> lessons;
        private TimetableUtils timetableUtils;
        private SettingsModel settingsModel;

        public TimetablePage() {
            InitializeComponent();
            //NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            //timetableUtils = new TimetableUtils();
            loadTimetables();
        }

        async void loadTimetables() {
            timetableModel = TimetableModel.getInstance();
            timetableUtils = new TimetableUtils();
            settingsModel = SettingsModel.getInstance();
            Lessons = await timetableModel.getTimetable(settingsModel.StgJhr, settingsModel.Stg, settingsModel.StgGrp);

            await updateLiveTile();

            /*find out if current week is even or odd*/
            int evenOdd = timetableUtils.isCurrentWeekEvenOrOdd();

            foreach (TimetableObject timetableObject in Lessons) {
                int row = timetableUtils.getRowForTable(timetableObject);
                if (row != -1) {
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
                    Grid.SetRow(timetableItem, row);
                    // switch for the week number
                    switch (timetableObject.Week) {
                        /*lesson takes place every week*/
                        case 0: {
                                addLessonToWeek(timetableThisWeek, row, timetableItem);
                                // clone the button
                                TimetableItem clone = new TimetableItem();
                                Grid.SetColumn(clone, timetableObject.Day);
                                Grid.SetRow(clone, row);
                                clone.TextBlock.Text = timetableItem.TextBlock.Text;
                                addLessonToWeek(timetableNextWeek, row, clone);
                                break;
                            }
                        /*only at odd weeks*/
                        case 1: {
                                /*if current week is even add it to next week*/
                                if (evenOdd == 0) {
                                    addLessonToWeek(timetableNextWeek, row, timetableItem);
                                }
                                else {
                                    addLessonToWeek(timetableThisWeek, row, timetableItem);
                                }
                                break;
                            }
                        /*only at even weeks*/
                        case 2: {
                                /*if current week is even add it to this week*/
                                if (evenOdd == 0) {
                                    addLessonToWeek(timetableThisWeek, row, timetableItem);
                                }
                                else {
                                    addLessonToWeek(timetableNextWeek, row, timetableItem);
                                }
                                break;
                            }
                    }
                }
            }
        }

        private void addLessonToWeek(Grid week, int row, TimetableItem timetableItem) {
            week.Children.Add(timetableItem);
            // TODO: show an indicator that more than one lesson are at the same time
    }



        // update the live tile
        private static async Task updateLiveTile() {
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

        public List<TimetableObject> Lessons {
            get {
                return lessons;
            }

            set {
                lessons = value;
            }
        }
    }
}
