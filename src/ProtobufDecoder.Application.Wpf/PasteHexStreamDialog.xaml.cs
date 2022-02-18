using System.Windows;

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
            HexString = HexStreamTextBlock.Text;
            DialogResult = true;
            Close();
        }

        public string HexString { get; set; }

        private void PasteHexStreamDialog_OnLoaded(object sender, RoutedEventArgs e)
        {
            HexStreamTextBlock.Focus();
        }
    }
}
