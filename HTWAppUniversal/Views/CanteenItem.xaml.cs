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
    public sealed partial class CanteenItem : Page {
        TextBlock tb_cat, tb_desc, tb_price;

        public CanteenItem() {
            this.InitializeComponent();
            Tb_cat = t_cat;
            Tb_desc = t_desc;
            Tb_price = t_price;
        }

        #region Properties
        public TextBlock Tb_cat {
            get { return tb_cat; }
            set { tb_cat = value; }
        }

        public TextBlock Tb_desc {
            get { return tb_desc; }
            set { tb_desc = value; }
        }

        public TextBlock Tb_price {
            get { return tb_price; }
            set { tb_price = value; }
        }
        #endregion
    }
}
