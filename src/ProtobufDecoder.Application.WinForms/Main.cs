using System;
using System.IO;
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

        private void selectProtobufFileDialog_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {
            textBoxFilePath.Text = selectProtobufFileDialog.FileName;
        }

        private void buttonBrowse_Click(object sender, EventArgs e)
        {
            selectProtobufFileDialog.ShowDialog(this);
        }

        private void buttonDecode_Click(object sender, EventArgs e)
        {
            var inputFilePath = textBoxFilePath.Text;
            
            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show(
                    "Please select a file to decode",
                    "No input file selected", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!File.Exists(inputFilePath))
            {
                MessageBox.Show(
                    "Please check the path to the input file",
                    "Input file does not exist", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            byte[] input;
            
            try
            {
                input = File.ReadAllBytes(inputFilePath);
            }
            catch (IOException ioException)
            {
                MessageBox.Show(
                    $"Reading the input file failed because: {ioException.Message}",
                    "Failed to read input file", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            ProtobufMessage protobufMessage;

            try
            {
                protobufMessage = ProtobufParser.Parse(input);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    $"Parsing the input file failed because: {exception.Message}",
                    "Failed to parse input file", 
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            foreach (var tag in protobufMessage.Tags)
            {
                var node = new TreeNode($"Tag {tag.Index}") { Tag = tag };

                treeView1.Nodes.Add(node);
            }
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            propertyGridTag.SelectedObject = null;
            propertyGridTag.Update();
            treeView1.Nodes.Clear();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            ProtobufTag tag = null;

            if (treeView1.SelectedNode != null)
            {
                tag = treeView1.SelectedNode.Tag as ProtobufTag;
            }

            propertyGridTag.SelectedObject = tag;
            propertyGridTag.Update();
        }
    }
}
