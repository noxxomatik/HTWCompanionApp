using HTWAppObjects;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

namespace HTWDDAppUniversal.Views
{
    public sealed partial class SettingsPage : Page
    {
        Template10.Services.SerializationService.ISerializationService _SerializationService;

        public SettingsPage() {
            InitializeComponent();
            _SerializationService = Template10.Services.SerializationService.SerializationService.Json;
            tb_sNr.MaxLength = 5;
            tb_sy.MaxLength = 2;
            tb_sg.MaxLength = 3;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e) {
            var index = int.Parse(_SerializationService.Deserialize(e.Parameter?.ToString()).ToString());
            MyPivot.SelectedIndex = index;
        }

        bool IsDigit(string s) {
            foreach (char c in s) {
                if (c < '0' || c > '9')
                    return false;
            }
            return true;
        }

        async void SaveData() {
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
            try {
                MessageDialog md = new MessageDialog("Einstellungen gespeichert");
                await md.ShowAsync();
            }
            catch (Exception e) {
                Debug.WriteLine(e.Message.ToString());
            }
        }

        private void b_save_Click(object sender, RoutedEventArgs e) {
            SaveData();
        }

        private void EnterPressed(object sender, Windows.UI.Xaml.Input.KeyRoutedEventArgs e) {
            if (e.Key == Windows.System.VirtualKey.Enter) {
                SaveData();
            }
        }

        private void tb_sNr_TextChanged(object sender, TextChangedEventArgs e) {
            if (tb_sNr.Text.Length != 5 || !IsDigit(tb_sNr.Text))
                tb_sNr.BorderBrush = new SolidColorBrush(Colors.Red);
            else
                tb_sNr.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void pb_pw_PasswordChanged(object sender, RoutedEventArgs e) {
            if (pb_pw.Password.Length == 0)
                pb_pw.BorderBrush = new SolidColorBrush(Colors.Red);
            else
                pb_pw.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void tb_sy_TextChanged(object sender, TextChangedEventArgs e) {
            if (tb_sy.Text.Length != 2 || !IsDigit(tb_sy.Text))
                tb_sy.BorderBrush = new SolidColorBrush(Colors.Red);
            else
                tb_sy.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void tb_sg_TextChanged(object sender, TextChangedEventArgs e) {
            if (tb_sg.Text.Length != 3 || !IsDigit(tb_sg.Text))
                tb_sg.BorderBrush = new SolidColorBrush(Colors.Red);
            else
                tb_sg.BorderBrush = new SolidColorBrush(Colors.Green);
        }

        private void tb_sgn_TextChanged(object sender, TextChangedEventArgs e) {
            if (tb_sgn.Text.Length == 0)
                tb_sgn.BorderBrush = new SolidColorBrush(Colors.Red);
            else
                tb_sgn.BorderBrush = new SolidColorBrush(Colors.Green);
        }
    }
}
