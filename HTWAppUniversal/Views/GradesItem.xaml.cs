using Windows.UI.Xaml.Controls;

namespace HTWAppUniversal.Views {
    /// <summary>
    /// Noten-Item aus Notenansicht (Liste)
    /// </summary>
    public sealed partial class GradesItem : Page {
        TextBlock tb_title, tb_grade, tb_credits;
        Grid gg;
        public GradesItem() {
            this.InitializeComponent();
            Tb_title = t_title;
            Tb_grade = t_grade;
            Tb_credits = t_credits;
            Gg = g_grid;
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

        public Grid Gg {
            get { return gg; }
            set { gg = value; }
        }
        #endregion
    }
}