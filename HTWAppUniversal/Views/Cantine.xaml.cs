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
    public sealed partial class Cantine : Page {
        public Cantine() {
            this.InitializeComponent();
            showData();
        }

        async void showData() {
            CanteenModel cm = CanteenModel.getInstance();
            List<CanteenObject> cantTdy = await cm.getCanteenToday();

            foreach (CanteenObject c in cantTdy) {
                // ListViewItem method (simple)
                /*ListViewItem lvi = new ListViewItem();
                string category = c.title.Split(':')[0];
                string content = c.title.Split(':')[1].Split('(')[0];
                string price = c.title.Split('(')[c.title.Split('(').Length - 1].Split(')')[0];
                lvi.Content = category + "\n" + content + "\n" + price;
                mainView.Items.Add(lvi);*/

                // custom page method
                CanteenItem ci = new CanteenItem();
                string category = c.title.Split(':')[0];
                string content = c.title.Split(':')[1].Split('(')[0];
                string price = c.title.Split('(')[c.title.Split('(').Length - 1].Split(')')[0];
                ci.Tb_cat.Text = category;
                ci.Tb_desc.Text = content;
                ci.Tb_price.Text = price;
                mainView.Items.Add(ci);
            }
        }
    }
}
