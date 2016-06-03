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
    public sealed partial class Grades : Page {
        public Grades() {
            this.InitializeComponent();
            showData();
        }

        async void showData() {
            SettingsModel sm = SettingsModel.getInstance();
            GradesModel gm = GradesModel.getInstance();

            /* for testing */
            sm.SNummer = "";
            sm.RZLogin = "";
            /* for testing */

            List<GradeObject> grades = await gm.getGrades(sm.SNummer, sm.RZLogin);

            // get all semesters
            List<string> semester = new List<string>();
            foreach (GradeObject g in grades) {
                if (!semester.Contains(g.semester))
                    semester.Add(g.semester);
            }

            // create as many listviews as there are semester
            List<ListView> semList = new List<ListView>();
            foreach (string s in semester) {
                ListView lv = new ListView();
                lv.Name = s;
                semList.Add(lv);
            }

            // todo: fill listviews with entries
            foreach (ListView lv in semList) {
                foreach (GradeObject g in grades) {
                    if (g.semester == lv.Name) {
                        // ListViewItem method (simple)
                        /*string grade = (double.Parse(g.prNote) / 100).ToString();
                        ListViewItem lvi = new ListViewItem();
                        lvi.Content = g.prTxt + "(" + g.prForm + ")" + "\n" + "Note: " + grade + "\t\t" + "Credits: " + g.ectsCredits;
                        lv.Items.Add(lvi);*/

                        // custom page method
                        double grade = double.Parse(g.prNote) / 100;
                        GradesItem gi = new GradesItem();
                        gi.Tb_title.Text = g.prTxt + "(" + g.prForm + ")";
                        gi.Tb_grade.Text = grade.ToString();
                        gi.Tb_credits.Text = g.ectsCredits;
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
        }
    }
}
