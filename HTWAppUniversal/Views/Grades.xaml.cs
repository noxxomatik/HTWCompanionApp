using HTWAppObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Notenansicht
    /// </summary>
    public sealed partial class Grades : Page {
        public Grades() {
            this.InitializeComponent();
            showData();
        }

        async void showData() {
            SettingsModel sm = SettingsModel.getInstance();
            GradesModel gm = GradesModel.getInstance();
            List<GradeObject> grades;

            // only show if correct user name/password
            try {
                grades = await gm.getGrades(sm.SNummer, sm.RZLogin);
            }
            catch (Exception e) {
                MessageDialog md = new MessageDialog("Ungültiger Benutzername und/oder Passwort in Einstellungen hinterlegt!");
                await md.ShowAsync();
                return;
            }

            // get all semesters
            List<string> semester = new List<string>();
            foreach (GradeObject g in grades) {
                if (!semester.Contains(g.Semester))
                    semester.Add(g.Semester);
            }

            // create as many listviews as there are semester
            List<ListView> semList = new List<ListView>();
            foreach (string s in semester) {
                ListView lv = new ListView();
                lv.Name = s;
                semList.Add(lv);
            }

            // fill listviews with entries
            foreach (ListView lv in semList) {
                int i = 0;
                foreach (GradeObject g in grades) {
                    if (g.Semester == lv.Name) {
                        // custom page as ListViewItem template
                        double grade = double.Parse(g.PrNote) / 100;
                        GradesItem gi = new GradesItem();
                        gi.Tb_title.Text = g.PrTxt + "(" + g.PrForm + ")";
                        gi.Tb_grade.Text = grade.ToString();
                        gi.Tb_credits.Text = g.EctsCredits;
                        if (i % 2 == 1)
                            gi.Gg.Background = new SolidColorBrush(Colors.LightGray);
                        gi.Tb_title.Width = this.ActualWidth * .5;
                        lv.Items.Add(gi);
                        i++;
                    }
                }
            }

            // add listviews to page
            foreach (ListView lv in semList) {
                ListViewItem lvi = new ListViewItem();
                string year = lv.Name.Substring(0, 4);
                string yearPart = lv.Name.Substring(4, 1);
                if (yearPart.Equals("1"))
                    lvi.Content = "Sommersemester " + year;
                else if (yearPart.Equals("2"))
                    lvi.Content = "Wintersemester " + year;
                lvi.Background = new SolidColorBrush(Colors.DarkGray);
                mainView.Items.Add(lvi);
                mainView.Items.Add(lv);
            }
            mainView.IsItemClickEnabled = true;
            mainView.ItemClick += new ItemClickEventHandler(ItemClick);
        }

        void ItemClick(object sender, ItemClickEventArgs e) {
            try {
                ListView lv = (ListView)sender;
                ListViewItem lvi = new ListViewItem();// = (ListViewItem)lv.FindName(e.ClickedItem.ToString());
                foreach (ListViewItem l in lv.Items) {
                    if (l.Content.ToString() == e.ClickedItem.ToString()) {
                        lvi = l;
                        break;
                    }
                }
                if (lvi.Content.ToString().Contains("Sommersemester") || lvi.Content.ToString().Contains("Wintersemester")) {
                    ListView tmp = (ListView)lv.Items[lv.Items.IndexOf(lvi) + 1];
                    if (tmp.Visibility == Windows.UI.Xaml.Visibility.Visible)
                        tmp.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    else
                        tmp.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            catch (Exception ex) {

            }
        }
    }
}
