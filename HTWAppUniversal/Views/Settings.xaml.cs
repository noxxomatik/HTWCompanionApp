using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Popups;
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
    public sealed partial class Settings : Page {
        public Settings() {
            this.InitializeComponent();
            tb_sNr.MaxLength = 5;
            tb_sy.MaxLength = 2;
            tb_sg.MaxLength = 3;
        }

        private void b_save_Click(object sender, RoutedEventArgs e) {
            SettingsModel sm = SettingsModel.getInstance();
            if (tb_sNr.Text.Length == 5)
                sm.SNummer = 's' + tb_sNr.Text;
            if (pb_pw.Password.Length != 0)
                sm.RZLogin = pb_pw.Password;
            if (tb_sy.Text.Length == 2)
                sm.StgJhr = tb_sy.Text;
            if (tb_sg.Text.Length == 3)
                sm.Stg = tb_sg.Text;
            if (tb_sgn.Text.Length != 0)
                sm.StgGrp = tb_sgn.Text;
        }

        private void tb_sNr_GotFocus(object sender, RoutedEventArgs e) {
            tb_sNr.Text = "";
        }

        private async void tb_sNr_LostFocus(object sender, RoutedEventArgs e) {
            if (tb_sNr.Text.Length != 5) {
                MessageDialog md = new MessageDialog("Ungültige s-Nummer");
                await md.ShowAsync();
                tb_sNr.Focus(FocusState.Programmatic);
            }
        }

        private void pb_pw_GotFocus(object sender, RoutedEventArgs e) {
            pb_pw.Password = "";
        }

        private async void pb_pw_LostFocus(object sender, RoutedEventArgs e) {
            if (pb_pw.Password.Length == 0) {
                MessageDialog md = new MessageDialog("Bitte Passwort eingeben");
                await md.ShowAsync();
                tb_sNr.Focus(FocusState.Programmatic);
            }

        }

        private void tb_sy_GotFocus(object sender, RoutedEventArgs e) {
            tb_sy.Text = "";
        }

        private async void tb_sy_LostFocus(object sender, RoutedEventArgs e) {
            if (tb_sy.Text.Length != 2) {
                MessageDialog md = new MessageDialog("Ungültiger Studienjahrgang");
                await md.ShowAsync();
                tb_sNr.Focus(FocusState.Programmatic);
            }
        }

        private void tb_sg_GotFocus(object sender, RoutedEventArgs e) {
            tb_sg.Text = "";
        }

        private async void tb_sg_LostFocus(object sender, RoutedEventArgs e) {
            if (tb_sg.Text.Length != 3) {
                MessageDialog md = new MessageDialog("Ungültiger Studiengang");
                await md.ShowAsync();
                tb_sNr.Focus(FocusState.Programmatic);
            }
        }

        private void tb_sgn_GotFocus(object sender, RoutedEventArgs e) {
            tb_sgn.Text = "";
        }

        private void tb_sgn_LostFocus(object sender, RoutedEventArgs e) {
            // keine pruefung
        }
    }
}
