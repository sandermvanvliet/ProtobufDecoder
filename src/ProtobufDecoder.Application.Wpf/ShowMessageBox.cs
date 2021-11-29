using System;
using System.IO;
using System.Windows;

namespace ProtobufDecoder.Application.Wpf
{
    public class ShowMessageBox
    {
        public static MessageBoxResult ForReadingInputFailed(IOException ioException)
        {
            return MessageBox.Show(
                string.Format(Strings.FailedToReadInput_Text, ioException.Message),
                Strings.FailedToReadInput_Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static MessageBoxResult ForFileDoesNotExist()
        {
            return MessageBox.Show(
                Strings.InputFileDoesNotExist_Text,
                Strings.FailedToReadInput_Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static MessageBoxResult ForNoInputSelected()
        {
            return MessageBox.Show(
                Strings.NoInputSelected_Text,
                Strings.NoInputSelected_Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static MessageBoxResult ForFailedToParseInput(Exception exception)
        {
            return MessageBox.Show(
                string.Format(Strings.FailedToParseInput_Text, exception.Message),
                Strings.FailedToParseInput_Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static MessageBoxResult ForFailedToDecodeTag(Exception exception)
        {
            return MessageBox.Show(
                string.Format(Strings.FailedToDecodeTag_Text, exception.Message),
                Strings.FailedToDecodeTag_Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }

        public static MessageBoxResult ForTagDecodingNotSupported()
        {
            return MessageBox.Show(
                Strings.CannotDecodeTag_Text,
                Strings.CannotDecodeTag_Caption,
                MessageBoxButton.OK,
                MessageBoxImage.Warning);
        }
    }
}