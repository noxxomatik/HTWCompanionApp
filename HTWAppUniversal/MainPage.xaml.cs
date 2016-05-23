﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Die Vorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409 dokumentiert.

namespace HTWAppUniversal
{
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }

        private async void textBlock_Loading(FrameworkElement sender, object args) {
            // TEST
            SettingsModel settings = new SettingsModel();
            // replace with your sNummer and password
            settings.SNummer = "sXXXXX";
            settings.RZLogin = "XXXX";
            settings.StgJhr = "15";
            settings.Stg = "044";
            settings.StgGrp = "73-CM";
            List<string> rooms = new List<string>();
            rooms.Add("S 354");
            rooms.Add("Z 701");
            rooms.Add("S 355");
            settings.Rooms = rooms;

            TimetableModel timetable = new TimetableModel();
            List<TimetableObject> timetableObjects = await timetable.getTimetable(settings.StgJhr, settings.Stg, settings.StgGrp);
            RoomTimetableModel roomTimetable = new RoomTimetableModel();
            roomTimetable.getRoomTimetable(settings.Rooms[1]);
            GradesModel gradesModel = new GradesModel();
            gradesModel.getGrades(settings.SNummer, settings.RZLogin);
            CanteenModel canteenModel = new CanteenModel();
            List<CanteenObject> foodList = await canteenModel.getCanteenToday();
            canteenModel.getCanteenTomorrow();

            TextBlock textBlock = (TextBlock)sender;
            textBlock.Text = timetableObjects[0].lessonTag;
            textBlock.Text += "\n" + (foodList.Count > 0 ? foodList[0].title : "");
        }
    }
}
