﻿using HTWAppObjects;
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
    public sealed partial class Timetable : Page {
        private TimetableModel timetableModel;
        private List<TimetableObject> lessons;
        private Util util;

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
            util = new Util();
        }


        private async void Page_Loading(FrameworkElement sender, object args)
        {
            timetableModel = TimetableModel.getInstance();
            //if online
            SettingsModel model = SettingsModel.getInstance();
            Lessons = await timetableModel.getTimetable(model.StgJhr, model.Stg, model.StgGrp);


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
                                    if (null == firstChild.Name || !Util.STACKPANEL.Equals(firstChild.Name))
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
                                Grid.SetRow(copy, row + Util.totalNumberofLessons + 1);

                                //find out, if another lesson takes place at the same time / had already been positioned in grid
                                FrameworkElement firstChild2 = util.getChildOfGrid(timetableGrid, Grid.GetRow(copy), Grid.GetColumn(copy));
                                if (null == firstChild2) // no other lesson found
                                    timetableGrid.Children.Add(copy);
                                else
                                {
                                    if (null == firstChild2.Name || !Util.STACKPANEL.Equals(firstChild2.Name))
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
                                    Grid.SetRow(tb, row + Util.totalNumberofLessons + 1);
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
                                    Grid.SetRow(tb, row + Util.totalNumberofLessons + 1);
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
        }

    }
}