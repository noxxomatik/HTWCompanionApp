using HTWAppObjects;
using System;
using System.Collections.Generic;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;

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
                foreach (GradeObject g in grades) {
                    if (g.Semester == lv.Name) {
                        // custom page as ListViewItem template
                        double grade = double.Parse(g.PrNote) / 100;
                        GradesItem gi = new GradesItem();
                        gi.Tb_title.Text = g.PrTxt + "(" + g.PrForm + ")";
                        gi.Tb_grade.Text = grade.ToString();
                        gi.Tb_credits.Text = g.EctsCredits;
                        lv.Items.Add(gi);
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
                mainView.Items.Add(lvi);
                mainView.Items.Add(lv);
            }
            mainView.SelectionChanged += new SelectionChangedEventHandler(SelectionChanged);
        }

        void SelectionChanged(object sender, SelectionChangedEventArgs e) {
            ListView lv = (ListView)sender;
            if (lv.SelectedItem != null) {
                ListViewItem lvi = (ListViewItem)lv.SelectedItem;
                if (lvi.Content.ToString().Contains("Sommersemester") || lvi.Content.ToString().Contains("Wintersemester")) {
                    // want to hide instead of remove but theres no option for that (DataGridView can hide Rows)
                    //lv.Items.RemoveAt(lv.SelectedIndex + 1);
                }
            }
        }
    }
}
