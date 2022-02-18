using System.Windows;
using System.Windows.Input;

namespace ProtobufDecoder.Application.Wpf
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class PasteHexStreamDialog : Window
    {
        public PasteHexStreamDialog()
        {
            InitializeComponent();
        }

        private void LoadButton_OnClick(object sender, RoutedEventArgs e)
        {
            CloseWithSuccess();
        }

        private void CloseWithSuccess()
        {
            HexString = HexStreamTextBlock.Text;
            DialogResult = true;
            Close();
        }

        public string HexString { get; set; }

        private void PasteHexStreamDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            HexStreamTextBlock.Focus();
        }

        private void PasteHexStreamDialog_OnKeyUp(object sender, KeyEventArgs e)
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
