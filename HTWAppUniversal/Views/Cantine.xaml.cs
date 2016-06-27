using HTWAppObjects;
using System.Collections.Generic;
using Windows.UI.Xaml.Controls;

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Kantinenansicht
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
                // custom page as ListViewItem template
                CanteenItem ci = new CanteenItem();
                string category = c.Title.Contains(":") ? c.Title.Split(':')[0] : "";
                string content = c.Title.Contains(":") ? c.Title.Split(':')[1].Split('(')[0] : c.Title.Split('(')[0];
                string price = c.Title.Split('(')[c.Title.Split('(').Length - 1].Split(')')[0];
                ci.Tb_cat.Text = category;
                ci.Tb_desc.Text = content;
                ci.Tb_price.Text = price;
                mainView.Items.Add(ci);
            }
            mainView.HorizontalContentAlignment = Windows.UI.Xaml.HorizontalAlignment.Stretch;
        }
    }
}
