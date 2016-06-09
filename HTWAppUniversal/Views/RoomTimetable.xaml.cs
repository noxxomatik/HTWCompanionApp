using System;
using System.Collections.Generic;
using HTWAppObjects;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RoomTimetable : Page {
        private RoomTimetableModel roomTimetableModel;
        private Util util;

        public RoomTimetable()
        {
            this.InitializeComponent();
            SettingsModel settingsModel = SettingsModel.getInstance();
            roomTimetableModel = RoomTimetableModel.getInstance();
            roomChoiceComboBox.DataContext = settingsModel.Rooms;
            util = new Util();
        }

        private async void roomSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox combo = (ComboBox)sender;
            String newRoom = combo.SelectedItem.ToString();


            /*check if grid already contains elements (except days and times = 13)
              if yes: delete those elements 
             */

            List<UIElement> children = timetableGrid.Children.ToList<UIElement>();
            if (children.Count > 13)
            {
                List<UIElement> hasToBeDeleted = new List<UIElement>();
                foreach (UIElement child in children)
                {
                    if (Grid.GetColumn((FrameworkElement)child) != 0)
                    {
                        hasToBeDeleted.Add(child);
                    }
                }

                foreach (UIElement item in hasToBeDeleted)
                {
                    timetableGrid.Children.Remove(item);
                }
            }

            /*request list of timetableObjects*/
            List<TimetableObject> items = await roomTimetableModel.getRoomTimetable(newRoom);

            /*find out if current week is even or odd*/
            int evenOdd = util.isCurrentWeekEvenOrOdd();

            /*display objects in grid*/
            foreach (TimetableObject item in items)
            {
                int row = util.getRowForTable(item);
                if (row != -1)
                {
                    TextBlock tb = util.setupRoomTimetableTextBlock(item);

                    switch (item.Week)
                    {

                        /*lesson takes place every week*/
                        case 0:
                            {
                                Grid.SetRow(tb, row);
                                timetableGrid.Children.Add(tb);

                                TextBlock copy = util.setupRoomTimetableTextBlock(item);
                                Grid.SetRow(copy, row + Util.totalNumberofLessons + 1); //Add 1 because of empty row in view
                                Grid.SetColumn(copy, item.Day);
                                timetableGrid.Children.Add(copy);
                                break;
                            }

                        /*only at odd weeks*/
                        case 1:
                            {
                                if (evenOdd == 0) /*current week is even -> show it first*/
                                    Grid.SetRow(tb, row + Util.totalNumberofLessons + 1);
                                else
                                    Grid.SetRow(tb, row);
                                timetableGrid.Children.Add(tb);
                                break;
                            }

                        /*only at even weeks*/
                        case 2:
                            {
                                if (evenOdd == 0) /*current week is even -> show it first*/
                                    Grid.SetRow(tb, row);
                                else
                                    Grid.SetRow(tb, row + Util.totalNumberofLessons + 1);
                                timetableGrid.Children.Add(tb);
                                break;
                            }

                        default:
                            {
                                break;
                            }
                    }

                }
            }

        }
    }
}
