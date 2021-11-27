using System;
using System.IO;
using System.Windows.Forms;

namespace ProtobufDecoder.Application.WinForms
{
    public class ShowMessageBox
    {
        public static DialogResult ForReadingInputFailed(IOException ioException)
        {
            return MessageBox.Show(
                string.Format(Strings.FailedToReadInput_Text, ioException.Message),
                Strings.FailedToReadInput_Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static DialogResult ForFileDoesNotExist()
        {
            return MessageBox.Show(
                Strings.InputFileDoesNotExist_Text,
                Strings.FailedToReadInput_Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static DialogResult ForNoInputSelected()
        {
            return MessageBox.Show(
                Strings.NoInputSelected_Text,
                Strings.NoInputSelected_Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static DialogResult ForFailedToParseInput(Exception exception)
        {
            return MessageBox.Show(
                string.Format(Strings.FailedToParseInput_Text, exception.Message),
                Strings.FailedToParseInput_Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static DialogResult ForFailedToDecodeTag(Exception exception)
        {
            return MessageBox.Show(
                string.Format(Strings.FailedToDecodeTag_Text, exception.Message),
                Strings.FailedToDecodeTag_Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        public static DialogResult ForTagDecodingNotSupported()
        {
            return MessageBox.Show(
                Strings.CannotDecodeTag_Text,
                Strings.CannotDecodeTag_Caption,
                MessageBoxButtons.OK,
                MessageBoxIcon.Warning);
        }
    }
}