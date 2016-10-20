using System;
using HTWDDAppUniversal.ViewModels;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System.Collections.ObjectModel;
using HTWAppObjects;
using System.Collections.Generic;
using Windows.UI.Xaml.Media;
using Windows.UI;
using Windows.UI.Popups;
using System.Diagnostics;
using HTWAppObjects.Objects;
using System.Threading.Tasks;

namespace HTWDDAppUniversal.Views
{
    public sealed partial class GradesPage : Page
    {
        public GradesPage() {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            loadGradesBackup();
        }

        async Task loadGrades() {
            SettingsModel settingsModel = SettingsModel.getInstance();
            GradesModel gradesModel = GradesModel.getInstance();
            GradeObjectsList gradeObjectsList;

            // only show if correct user name/password
            try {
                gradeObjectsList = await gradesModel.getGrades(settingsModel.SNummer, settingsModel.RZLogin);
            }
            catch (Exception e) {
                Debug.WriteLine(e);
                MessageDialog md = new MessageDialog("Ungültiger Benutzername und/oder Passwort in den Einstellungen hinterlegt!");
                await md.ShowAsync();
                return;
            }

            if (gradeObjectsList != null) {
                // get all semesters
                List<string> semester = new List<string>();
                foreach (GradeObject gradeObject in gradeObjectsList.gradeObjects) {
                    if (!semester.Contains(gradeObject.Semester))
                        semester.Add(gradeObject.Semester);
                }

                // create as many listviews as there are semester
                List<ListView> semesterList = new List<ListView>();
                foreach (string s in semester) {
                    ListView listView = new ListView();
                    listView.Name = s;
                    semesterList.Add(listView);
                }

                // fill listviews with entries
                foreach (ListView listView in semesterList) {
                    int i = 0;
                    foreach (GradeObject gradeObject in gradeObjectsList.gradeObjects) {
                        if (gradeObject.Semester == listView.Name) {
                            // custom page as ListViewItem template
                            double grade = double.Parse(gradeObject.PrNote) / 100;
                            GradesItem gradesItem = new GradesItem();
                            gradesItem.Tb_title.Text = gradeObject.PrTxt + " (" + gradeObject.PrForm + ")";
                            gradesItem.Tb_grade.Text = grade.ToString();
                            gradesItem.Tb_credits.Text = gradeObject.EctsCredits;
                            /*if (i % 2 == 1)
                                gradesItem.Gg.Background = new SolidColorBrush(Colors.LightGray);
                            gradesItem.Tb_title.Width = this.ActualWidth * .75;*/
                            listView.Items.Add(gradesItem);
                            i++;
                        }
                    }
                }

                // clear listview
                clearListView(mainView);

                // add listviews to page
                foreach (ListView listView in semesterList) {
                    ListViewItem listViewHeader = new ListViewItem();
                    string year = listView.Name.Substring(0, 4);
                    string yearPart = listView.Name.Substring(4, 1);
                    if (yearPart.Equals("1"))
                        listViewHeader.Content = "Sommersemester " + year;
                    else if (yearPart.Equals("2"))
                        listViewHeader.Content = "Wintersemester " + year;
                    listViewHeader.Background = new SolidColorBrush(Colors.LightSteelBlue);
                    mainView.Items.Add(listViewHeader);
                    ListViewItem listViewContent = new ListViewItem();
                    listViewContent.Content = listView;
                    mainView.Items.Add(listViewContent);
                }

                mainView.IsItemClickEnabled = true;
                mainView.ItemClick += new ItemClickEventHandler(itemClick);

                timestamp.Text = "Stand: " + gradeObjectsList.timestamp.ToLocalTime().ToString();

                collapseOldEntries();
            }
        }

        async void loadGradesBackup() {
            SettingsModel settingsModel = SettingsModel.getInstance();
            GradesModel gradesModel = GradesModel.getInstance();
            GradeObjectsList gradeObjectsList;

            // only show if correct user name/password
            try {
                gradeObjectsList = await gradesModel.loadGradesBackup(settingsModel.SNummer);
            }
            catch (Exception e) {
                Debug.WriteLine(e);
                MessageDialog md = new MessageDialog("Ungültiger Benutzername und/oder Passwort in den Einstellungen hinterlegt!");
                await md.ShowAsync();
                return;
            }

            if (gradeObjectsList != null) {
                // get all semesters
                List<string> semester = new List<string>();
                foreach (GradeObject gradeObject in gradeObjectsList.gradeObjects) {
                    if (!semester.Contains(gradeObject.Semester))
                        semester.Add(gradeObject.Semester);
                }

                // create as many listviews as there are semester
                List<ListView> semesterList = new List<ListView>();
                foreach (string s in semester) {
                    ListView listView = new ListView();
                    listView.Name = s;
                    semesterList.Add(listView);
                }

                // fill listviews with entries
                foreach (ListView listView in semesterList) {
                    int i = 0;
                    foreach (GradeObject gradeObject in gradeObjectsList.gradeObjects) {
                        if (gradeObject.Semester == listView.Name) {
                            // custom page as ListViewItem template
                            double grade = double.Parse(gradeObject.PrNote) / 100;
                            GradesItem gradesItem = new GradesItem();
                            gradesItem.Tb_title.Text = gradeObject.PrTxt + " (" + gradeObject.PrForm + ")";
                            gradesItem.Tb_grade.Text = grade.ToString();
                            gradesItem.Tb_credits.Text = gradeObject.EctsCredits;
                            /*if (i % 2 == 1)
                                gradesItem.Gg.Background = new SolidColorBrush(Colors.LightGray);
                            gradesItem.Tb_title.Width = this.ActualWidth * .75;*/
                            listView.Items.Add(gradesItem);
                            i++;
                        }
                    }
                }

                // clear listview
                clearListView(mainView);

                // add listviews to page
                foreach (ListView listView in semesterList) {
                    ListViewItem listViewHeader = new ListViewItem();
                    string year = listView.Name.Substring(0, 4);
                    string yearPart = listView.Name.Substring(4, 1);
                    if (yearPart.Equals("1"))
                        listViewHeader.Content = "Sommersemester " + year;
                    else if (yearPart.Equals("2"))
                        listViewHeader.Content = "Wintersemester " + year;
                    listViewHeader.Background = new SolidColorBrush(Colors.LightSteelBlue);
                    mainView.Items.Add(listViewHeader);
                    ListViewItem listViewContent = new ListViewItem();
                    listViewContent.Content = listView;
                    mainView.Items.Add(listViewContent);
                }

                mainView.IsItemClickEnabled = true;
                mainView.ItemClick += new ItemClickEventHandler(itemClick);

                timestamp.Text = "Stand: " + gradeObjectsList.timestamp.ToLocalTime().ToString();

                collapseOldEntries();
            }
        }

        private void collapseOldEntries() {
            for (int i = 1; i < mainView.Items.Count - 2; i = i+2) {
                ((ListViewItem) mainView.Items[i]).Visibility = Visibility.Collapsed;
            }
        }

        void clearListView(ListView listView) {
            listView.Items.Clear();
            listView.ItemClick -= new ItemClickEventHandler(itemClick);
        }

        void itemClick(object sender, ItemClickEventArgs e) 
        {
            try {
                ListView listView = (ListView) sender;
                ListViewItem listViewItem = new ListViewItem(); // = (ListViewItem)lv.FindName(e.ClickedItem.ToString());
                foreach (ListViewItem l in listView.Items) {
                    if (l.Content.ToString() == e.ClickedItem.ToString()) {
                        listViewItem = l;
                        break;
                    }
                }
                if (listViewItem.Content.ToString().Contains("Sommersemester") || listViewItem.Content.ToString().Contains("Wintersemester")) {
                    ListViewItem tmp = (ListViewItem) listView.Items[listView.Items.IndexOf(listViewItem) + 1];
                    if (tmp.Visibility == Windows.UI.Xaml.Visibility.Visible)
                        tmp.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                    else
                        tmp.Visibility = Windows.UI.Xaml.Visibility.Visible;
                }
            }
            catch (Exception ex) {
                Debug.WriteLine(ex);
            }
        }

        private async void symbolIcon_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e) {
            symbolIcon.Visibility = Visibility.Collapsed;
            progressRing.Visibility = Visibility.Visible;

            await loadGrades();

            progressRing.Visibility = Visibility.Collapsed;
            symbolIcon.Visibility = Visibility.Visible;
        }
    }
}
