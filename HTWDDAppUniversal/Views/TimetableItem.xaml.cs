using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace HTWDDAppUniversal.Views
{
    public sealed partial class TimetableItem : Page
    {
        Button button;
        TextBlock textBlock;
        public TimetableItem() {
            this.InitializeComponent();

            Button = lesson;
            TextBlock = text;
        }

        void lessonButton_Click(object sender, RoutedEventArgs e) {
            //var button = sender as Button;
            //var theValue = button.Attributes["myParam"].ToString();
            Debug.WriteLine("Lesson clicked");
        }

        public Button Button {
            get {
                return button;
            }

            set {
                button = value;
            }
        }

        public TextBlock TextBlock {
            get {
                return textBlock;
            }

            set {
                textBlock = value;
            }
        }
    }
}
