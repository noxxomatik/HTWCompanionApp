using HTWAppObjects;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Data.Xml.Dom;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// Die Elementvorlage "Leere Seite" ist unter http://go.microsoft.com/fwlink/?LinkId=234238 dokumentiert.

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Eine leere Seite, die eigenständig verwendet oder zu der innerhalb eines Rahmens navigiert werden kann.
    /// </summary>
    public sealed partial class Timetable : Page {
        private TimetableModel timetableModel;
        private List<TimetableObject> lessons;
        private TimetableUtils util;

        public List<TimetableObject> Lessons
        {
            get
            {
                return lessons;
            }

            set
            {
                lessons = value;
            }
        }

        public Timetable()
        {
            this.InitializeComponent();
            util = new TimetableUtils();
        }


        private async void Page_Loading(FrameworkElement sender, object args)
        {
            timetableModel = TimetableModel.getInstance();
            // if online
            SettingsModel model = SettingsModel.getInstance();
            Lessons = await timetableModel.getTimetable(model.StgJhr, model.Stg, model.StgGrp);

            // update the live tile
            try {
                // Load the item.
                XmlDocument tileXml = await TimetableModel.GetNextLessonXml();
                if (tileXml == null)
                    throw new Exception("Timetable is empty.");
                // Update the live tile with the item.
                TimetableModel.UpdateTile(tileXml);
            }
            catch {
                Debug.WriteLine("Missing timteable item. First load the timetable.");
            }

            /*find out if current week is even or odd*/
            int evenOdd = util.isCurrentWeekEvenOrOdd();

            /*display objects in grid*/
            foreach (TimetableObject item in Lessons)
            {
                int row = util.getRowForTable(item);
                if (row != -1)
                {
                    TextBlock tb = util.setupTimetableTextBlock(item);

                    switch (item.Week)
                    {

                        /*lesson takes place every week*/
                        case 0:
                            {
                                Grid.SetRow(tb, row);

                                //find out, if another lesson takes place at the same time / had already been positioned in grid
                                FrameworkElement firstChild = util.getChildOfGrid(timetableGrid, row, Grid.GetColumn(tb));
                                if (null == firstChild) // no other lesson found
                                    timetableGrid.Children.Add(tb);
                                else
                                {
                                    if (null == firstChild.Name || !TimetableUtils.STACKPANEL.Equals(firstChild.Name))
                                    { //other lesson found
                                        StackPanel stackpanel = util.createStackPanel(timetableGrid, firstChild, tb);
                                        Grid.SetRow(stackpanel, row);
                                        timetableGrid.Children.Add(stackpanel);
                                    }
                                    else // more than one lessons found
                                    {
                                        ((StackPanel)firstChild).Children.Add(tb);
                                    }
                                }


                                TextBlock copy = util.setupTimetableTextBlock(item);
                                Grid.SetRow(copy, row + TimetableUtils.totalNumberofLessons + 1);

                                //find out, if another lesson takes place at the same time / had already been positioned in grid
                                FrameworkElement firstChild2 = util.getChildOfGrid(timetableGrid, Grid.GetRow(copy), Grid.GetColumn(copy));
                                if (null == firstChild2) // no other lesson found
                                    timetableGrid.Children.Add(copy);
                                else
                                {
                                    if (null == firstChild2.Name || !TimetableUtils.STACKPANEL.Equals(firstChild2.Name))
                                    { //other lesson found
                                        StackPanel stackpanel = util.createStackPanel(timetableGrid, firstChild2, copy);
                                        Grid.SetRow(stackpanel, Grid.GetRow(copy));
                                        timetableGrid.Children.Add(stackpanel);
                                    }
                                    else // more than one lessons found
                                    {
                                        ((StackPanel)firstChild).Children.Add(copy);
                                    }
                                }
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
            scrollViewer.MaxHeight = this.ActualHeight - 100;
        }
    }
}