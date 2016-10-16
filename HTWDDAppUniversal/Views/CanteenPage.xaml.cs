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

namespace HTWDDAppUniversal.Views
{
    public sealed partial class CanteenPage : Page
    {
        public CanteenPage() {
            InitializeComponent();
            NavigationCacheMode = Windows.UI.Xaml.Navigation.NavigationCacheMode.Enabled;
            showData();
        }

        void showData() {
            showCanteenToday();
            showCanteenTomorrow();
        }

        async void showCanteenToday() {
            CanteenModel cm = CanteenModel.getInstance();
            List<CanteenObject> canteenToday = await cm.getCanteenToday();

            int i = 0;
            if (canteenToday.Count > 0) {
                foreach (CanteenObject c in canteenToday) {
                    // custom page as ListViewItem template
                    CanteenItem ci = new CanteenItem();
                    string category = c.Title.Contains(":") ? c.Title.Split(':')[0] : "";
                    if (category == "")
                        //ci.Tb_cat.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        category = "Kantine";
                    string content = c.Title.Contains(":") ? c.Title.Split(':')[1].Split('(')[0] : c.Title.Split('(')[0];
                    string price = c.Title.Split('(')[c.Title.Split('(').Length - 1].Split(')')[0];
                    ci.Tb_cat.Text = category;
                    ci.Tb_desc.Text = content;
                    ci.Tb_price.Text = price;
                    if (i % 2 == 1)
                        ci.Gg.Background = new SolidColorBrush(Colors.LightGray);
                    ci.Tb_desc.Width = this.ActualWidth * .75;
                    mainViewToday.Items.Add(ci);
                    i++;
                }
                InformationToday.Text = "";
            }
            else {
                InformationToday.Text = "Keine Angebote an diesem Tag.";
            }

            mainViewToday.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        }

        async void showCanteenTomorrow() {
            CanteenModel cm = CanteenModel.getInstance();
            List<CanteenObject> canteenTomorrow = await cm.getCanteenTomorrow();

            int i = 0;
            if (canteenTomorrow.Count > 0) {
                foreach (CanteenObject c in canteenTomorrow) {
                    // custom page as ListViewItem template
                    CanteenItem ci = new CanteenItem();
                    string category = c.Title.Contains(":") ? c.Title.Split(':')[0] : "";
                    if (category == "")
                        //ci.Tb_cat.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                        category = "Kantine";
                    string content = c.Title.Contains(":") ? c.Title.Split(':')[1].Split('(')[0] : c.Title.Split('(')[0];
                    string price = c.Title.Split('(')[c.Title.Split('(').Length - 1].Split(')')[0];
                    ci.Tb_cat.Text = category;
                    ci.Tb_desc.Text = content;
                    ci.Tb_price.Text = price;
                    if (i % 2 == 1)
                        ci.Gg.Background = new SolidColorBrush(Colors.LightGray);
                    ci.Tb_desc.Width = this.ActualWidth * .75;
                    mainViewTomorrow.Items.Add(ci);
                    i++;
                }
                InformationTomorrow.Text = "";
            }
            else {
                InformationTomorrow.Text = "Keine Angebote an diesem Tag.";
            }

            mainViewTomorrow.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        }
    }
}
