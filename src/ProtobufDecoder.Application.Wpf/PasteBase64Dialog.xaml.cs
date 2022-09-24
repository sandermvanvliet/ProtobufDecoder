using System.Windows;
using System.Windows.Input;

namespace ProtobufDecoder.Application.Wpf
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class PasteBase64Dialog : Window
    {
        public PasteBase64Dialog()
        {
            InitializeComponent();
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            CloseWithSuccess();
        }

        private void CloseWithSuccess()
        {
            Base64String = Base64TextBlock.Text;
            DialogResult = true;
            Close();
        }

        public string Base64String { get; set; }

        private void PasteBase64Dialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            Base64TextBlock.Focus();
        }

        private void PasteBase64Dialog_OnKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                e.Handled = true;
                DialogResult = false;
                Close();
            }
            else if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                e.Handled = true;
                CloseWithSuccess();
            }
        }
    }
}
