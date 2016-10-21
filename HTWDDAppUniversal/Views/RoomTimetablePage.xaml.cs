using HTWAppObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using Windows.System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HTWDDAppUniversal.Views
{
    public sealed partial class RoomTimetablePage : Page
    {
        private RoomTimetableModel roomTimetableModel;
        private TimetableUtils timetableUtils;
        private SettingsModel settingsModel;

        private ObservableCollection<String> rooms;

        private bool suggestionChosen = false;

        public RoomTimetablePage()
        {
            this.InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            settingsModel = SettingsModel.GetInstance();
            rooms = new ObservableCollection<string>(settingsModel.Rooms);
            roomTimetableModel = RoomTimetableModel.GetInstance();
            timetableUtils = new TimetableUtils();
        }

        private async void SetupTimetable(string newRoom)
        {
            // clear timetable
            timetableUtils.clearTimetable(timetableThisWeek);

            /*request list of timetableObjects*/
            List<TimetableObject> items = await roomTimetableModel.GetRoomTimetable(newRoom);

            /*find out if current week is even or odd*/
            int evenOdd = timetableUtils.isCurrentWeekEvenOrOdd();

            /*display objects in grid*/
            foreach (TimetableObject item in items) {
                int[] row = timetableUtils.getRowForTable(item);
                if (row[0] != -1 && row[1] != -1) {
                    for (int currentRow = row[0]; currentRow <= row[1]; currentRow++) {
                        // prepare the button for this lesson
                        TimetableItem timetableItem = new TimetableItem();
                        timetableItem.TextBlock.Text = item.LessonTag + "\n"
                            + item.Type + "\n"
                            + item.Professor;
                        Grid.SetColumn(timetableItem, item.Day);
                        Grid.SetRow(timetableItem, currentRow);

                        // switch for the week number
                        switch (item.Week) {
                            /*lesson takes place every week*/
                            case 0: {
                                    timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                    break;
                                }
                            /*only at odd weeks*/
                            case 1: {
                                    if (evenOdd == 1) {
                                        timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                    }
                                    break;
                                }
                            /*only at even weeks*/
                            case 2: {
                                    if (evenOdd == 0) /*current week is even -> show it first*/
                                        timetableUtils.addLessonToWeek(timetableThisWeek, currentRow, timetableItem);
                                    break;
                                }
                            default: {
                                    break;
                                }
                        }
                    }
                }
            }
        }

        private void SetRoom(String newRoom)
        {
            if (null != timetableUtils.checkRoomSpell(newRoom)) {
                if (!timetableUtils.lookupRoom(newRoom)) {
                    SuggestedRooms.Clear();
                    foreach (String room in settingsModel.Rooms) {
                        SuggestedRooms.Add(room);
                    }
                    SuggestedRooms.Add(newRoom);
                    settingsModel.Rooms = new List<String>(SuggestedRooms);
                }
                SetupTimetable(newRoom);
            }
        }

        private void RoomChoiceComboBoxTextBox_KeyUp(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter) {
                String newRoom = ((AutoSuggestBox) sender).Text;
                SetRoom(newRoom);
            }
        }

        // try to set the input as a room
        private void RoomChoiceComboBoxTextBox_LostFocus(object sender, RoutedEventArgs e)
        {
            String newRoom = ((AutoSuggestBox) sender).Text;
            SetRoom(newRoom);
        }
        private void RoomChoiceComboBoxTextBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs e)
        {
            String newRoom = sender.Text;
            SetRoom(newRoom);
        }
        private void RoomChoiceComboBoxTextBox_SuggestionChosen(AutoSuggestBox sender, AutoSuggestBoxSuggestionChosenEventArgs e)
        {
            suggestionChosen = true;
        }


        // show suggestion which match the input
        private void RoomChoiceComboBoxTextBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            String text = sender.Text.ToLower();
            // a suggestion was chosen and added to the edit field
            if (suggestionChosen) {
                SetRoom(text);
                suggestionChosen = false;
            }
            else {
                SuggestedRooms.Clear();

                List<string> all = new List<string>(settingsModel.Rooms);
                foreach (String room in all) {
                    String lower = room.ToLower();
                    Regex regex = new Regex(@"\b" + text);

                    if (regex.Match(lower).Success) {
                        SuggestedRooms.Add(room);
                    }
                }
            }
        }

        private ObservableCollection<String> SuggestedRooms
        {
            get { return rooms; }
            set { rooms = value; }

        }
    }
}
