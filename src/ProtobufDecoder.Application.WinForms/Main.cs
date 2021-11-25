using System;
using System.Windows.Forms;

namespace ProtobufDecoder.Application.WinForms
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void aboutProtobufDecoderToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var aboutForm = new About();

            aboutForm.ShowDialog(this);
        }
    }
}
