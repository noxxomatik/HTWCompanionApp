using System;
using System.Collections.Generic;
using HTWAppObjects;
using System.Linq;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class RoomTimetable : Page {
        private RoomTimetableModel roomTimetableModel;
        private TimetableUtils util;
        private SettingsModel settingsModel;

        private ObservableCollection<String> rooms;

        private ObservableCollection<String> SuggestedRooms
        {
            get { return rooms; }
            set { rooms = value; }
            
        }


        public RoomTimetable()
        {
            this.InitializeComponent();
            settingsModel = SettingsModel.getInstance();
            rooms = new ObservableCollection<string>(settingsModel.Rooms);
            roomTimetableModel = RoomTimetableModel.getInstance();
            util = new TimetableUtils();
        }

        private async void setupTimetable(string newRoom)
        {
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
                                Grid.SetRow(copy, row + TimetableUtils.totalNumberofLessons + 1); //Add 1 because of empty row in view
                                Grid.SetColumn(copy, item.Day);
                                timetableGrid.Children.Add(copy);
                                break;
                            }

                        /*only at odd weeks*/
                        case 1:
                            {
                                if (evenOdd == 0) /*current week is even -> show it first*/
                                    Grid.SetRow(tb, row + TimetableUtils.totalNumberofLessons + 1);
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
                                    Grid.SetRow(tb, row + TimetableUtils.totalNumberofLessons + 1);
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
            g.Height = this.ActualHeight;
            scrollViewer.MaxHeight = this.ActualHeight - 140;
        }





        private void roomChoiceComboBoxTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if(e.Key == VirtualKey.Enter)
            {
                String newRoom = ((AutoSuggestBox)sender).Text;
                setRoom(newRoom);
            }
        }

        private void roomChoiceComboBoxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            String newRoom = ((AutoSuggestBox)sender).Text;
            setRoom(newRoom);
        }

        private void setRoom(String newRoom)
        {

            if (null != util.checkRoomSpell(newRoom))
            {
                if (!util.lookupRoom(newRoom))
                {
                    SuggestedRooms.Clear();
                    foreach (String room in settingsModel.Rooms)
                    {
                        SuggestedRooms.Add(room);
                    }

                    SuggestedRooms.Add(newRoom);
                    settingsModel.Rooms = new List<String>(SuggestedRooms);
                }

                setupTimetable(newRoom);
            }

        }

        private async void editListButton_Click(object sender, RoutedEventArgs e)
        {
            ContentDialogEditRooms editRoomsDialog = new ContentDialogEditRooms(this.ActualWidth);
            var result = await editRoomsDialog.ShowAsync();

            if (result == ContentDialogResult.Primary)
            {
                SuggestedRooms = new ObservableCollection<String>(settingsModel.Rooms);
                roomChoiceComboBoxTextBox.ItemsSource = SuggestedRooms;
            }

        }

        private void roomChoiceComboBoxTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            String text = sender.Text.ToLower();
            SuggestedRooms.Clear();

            List<string> all = new List<string>(settingsModel.Rooms);
            foreach(String room in all)
            {
                String lower = room.ToLower();
                Regex regex = new Regex(@"\b" + text);

                if (regex.Match(lower).Success)
                {
                    SuggestedRooms.Add(room);
                }
            }
            
        }


    }
}