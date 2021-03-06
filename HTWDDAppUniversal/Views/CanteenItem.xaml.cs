﻿using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;

namespace HTWDDAppUniversal.Views
{
    /// <summary>
    /// Kantinen-Item aus Kantinenansicht (Liste)
    /// </summary>
    public sealed partial class CanteenItem : Page
    {
        TextBlock tb_cat, tb_desc, tb_price;
        Hyperlink hyperlink;
        Grid gg;

        public CanteenItem()
        {
            this.InitializeComponent();
            Tb_cat = t_cat;
            Tb_desc = t_desc;
            Tb_price = t_price;
            Gg = g_grid;
            Hyperlink = t_hyperlink;
        }

        #region Properties
        public TextBlock Tb_cat
        {
            get { return tb_cat; }
            set { tb_cat = value; }
        }

        public TextBlock Tb_desc
        {
            get { return tb_desc; }
            set { tb_desc = value; }
        }

        public TextBlock Tb_price
        {
            get { return tb_price; }
            set { tb_price = value; }
        }

        public Grid Gg
        {
            get { return gg; }
            set { gg = value; }
        }

        public Hyperlink Hyperlink
        {
            get {
                return hyperlink;
            }

            set {
                hyperlink = value;
            }
        }
        #endregion
    }
}
