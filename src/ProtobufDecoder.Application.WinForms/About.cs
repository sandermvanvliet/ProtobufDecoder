using System;
using System.Windows.Forms;

namespace ProtobufDecoder.Application.WinForms
{
    public partial class About : Form
    {
        public About()
        {
            InitializeComponent();

            labelAppVersion.Text = this.GetType().Assembly.GetName().Version.ToString();
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
