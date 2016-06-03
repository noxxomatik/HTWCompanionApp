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
    public sealed partial class GradesItem : Page {
        TextBlock tb_title, tb_grade, tb_credits;
        public GradesItem() {
            this.InitializeComponent();
            Tb_title = t_title;
            Tb_grade = t_grade;
            Tb_credits = t_credits;
        }

        #region Properties
        public TextBlock Tb_credits {
            get { return tb_credits; }
            set { tb_credits = value; }
        }

        public TextBlock Tb_grade {
            get { return tb_grade; }
            set { tb_grade = value; }
        }

        public TextBlock Tb_title {
            get { return tb_title; }
            set { tb_title = value; }
        }
        #endregion
    }
}