using System;
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

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Shell : Page {
        public Shell() {
            this.InitializeComponent();
        }

        private void b_back_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
                if (frame?.CanGoBack == true)
                    frame.GoBack();
        }

        private void b_hamburger_Click(object sender, RoutedEventArgs e) {
            if (!this.SplitView.IsPaneOpen) {
                this.SplitView.IsPaneOpen = true;
            }
        }

        private void b_home_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(Views.MainPage)) {
                frame.Navigate(typeof(Views.MainPage));
            }
        }

        private void b_settings_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(Views.Settings)) {
                frame.Navigate(typeof(Views.Settings));
            }
        }

        private void b_grades_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(Views.Grades)) {
                frame.Navigate(typeof(Views.Grades));
            }
        }

        private void b_cantine_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(Views.Cantine)) {
                frame.Navigate(typeof(Views.Cantine));
            }
        }

        private void b_timetable_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(Views.Timetable)) {
                frame.Navigate(typeof(Views.Timetable));
            }
        }

        private void b_rTimetable_Click(object sender, RoutedEventArgs e) {
            var frame = this.DataContext as Frame;
            Page page = frame?.Content as Page;
            if (page?.GetType() != typeof(Views.RoomTimetable)) {
                frame.Navigate(typeof(Views.RoomTimetable));
            }
        }
    }
}
