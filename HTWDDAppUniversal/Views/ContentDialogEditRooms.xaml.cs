using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.System;
using System.Collections.ObjectModel;
using HTWAppObjects;
using HTWDDAppUniversal;

// The Content Dialog item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace HTWDDAppUniversal
{
    public sealed partial class ContentDialogEditRooms : ContentDialog
    {

        private SettingsModel settingsModel;
        private List<String> deletedRooms; //store checked items in seperate list
        private TimetableUtils util;
        private ObservableCollection<String> listViewItems; //Binding property for listView

        public ObservableCollection<String> ListViewItems
        {
            get { return listViewItems; }
            set { listViewItems = value; }
        }

        public ContentDialogEditRooms(double maxWidth)
        {
            this.InitializeComponent();
            MaxWidth = maxWidth; // required for mobile
            FullSizeDesired = true;
            
            settingsModel = SettingsModel.getInstance();
            listViewItems = new ObservableCollection<String>(settingsModel.Rooms);
            
            deletedRooms = new List<String>();
            util = new TimetableUtils();
        }

        private void ContentDialog_PrimaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            /*publish new listitems*/
            settingsModel.Rooms = new List<String>(listViewItems);
        }

        private void ContentDialog_SecondaryButtonClick(ContentDialog sender, ContentDialogButtonClickEventArgs args)
        {
            //nothing to do
        }

        /*when was enter pressed, check if value is correct and store it in listView*/
        private void addRoomTextBox_KeyUp(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == VirtualKey.Enter)
            {
                String room = util.checkRoomSpell(addRoomTextBox.Text);
                if (null != room)
                {
                    listViewItems.Add(room);
                    roomsList.ScrollIntoView(room);
                }
            }
        }


        /*if user pressed the button, check user input and store it in listView*/
        private void addRoomButton_Click(object sender, RoutedEventArgs e)
        {
            String room = util.checkRoomSpell(addRoomTextBox.Text);
            if (null != room)
            {
                listViewItems.Add(room);
                roomsList.ScrollIntoView(room);
            }
        }


        /*event handler for (un)checking*/
        private void onCheckBoxUnchecked(object sender, RoutedEventArgs e)
        {
            findTextToCheckbox((CheckBox)sender, false);
        }

        private void onCheckboxChecked(object sender, RoutedEventArgs e)
        {
            findTextToCheckbox((CheckBox)sender, true);
        }


        /*find the appropriate textblock to checkbox and add/delete it*/
        private void findTextToCheckbox(CheckBox checkbox, bool addToDeletedRooms)
        {
            StackPanel parent = (StackPanel)checkbox.Parent;
            foreach(UIElement uiElement in parent.Children)
            {
                FrameworkElement frameworkElement = (FrameworkElement)uiElement;
                if (frameworkElement.Name.Equals("room"))
                {
                    TextBlock textblock = (TextBlock)frameworkElement;
                    /*if checkbox had been checked add item to seperate list*/
                    if (addToDeletedRooms)
                        deletedRooms.Add(textblock.Text);
                    /*if checkbox hab been unchecked remove item from list*/
                    else
                        deletedRooms.Remove(textblock.Text);
                }
            }
        }


        /*delete all items of seperate list from listview*/
        private void deleteSelected_Click(object sender, RoutedEventArgs e)
        {
            foreach (String item in deletedRooms)
            {
                listViewItems.Remove(item);
            }

            deletedRooms.Clear();
        }


        /*setup sizes*/
        private void ContentDialog_Loaded(object sender, RoutedEventArgs e)
        {
            addRoomTextBox.Width = (layoutGrid.ActualWidth / 2) - addRoomButton.Width - addRoomPanel.Margin.Right;
            deleteSelected.Width = addRoomTextBox.Width + addRoomButton.Width;
        }

        private void roomsList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
