using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace ProtobufDecoder.Application.WinForms
{
    public partial class Main : Form
    {
        private ProtobufMessage _protobufMessage;
        private bool _isSelectingFromTreeView;

        public Main()
        {
            InitializeComponent();
        }

        public Main(byte[] input) : this()
        {
            Decode(input);
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
            // Ensure everything is cleared before we add something new.
            buttonClear.PerformClick();

            var inputFilePath = textBoxFilePath.Text;

            if (string.IsNullOrEmpty(inputFilePath))
            {
                MessageBox.Show(
                    Strings.NoInputSelected_Text,
                    Strings.NoInputSelected_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            if (!File.Exists(inputFilePath))
            {
                MessageBox.Show(
                    Strings.InputFileDoesNotExist_Text,
                    Strings.FailedToReadInput_Caption,
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
                    string.Format(Strings.FailedToReadInput_Text, ioException.Message),
                    Strings.FailedToReadInput_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            Decode(input);
        }

        private void Decode(byte[] input)
        {
            try
            {
                _protobufMessage = ProtobufParser.Parse(input);
            }
            catch (Exception exception)
            {
                MessageBox.Show(
                    string.Format(Strings.FailedToParseInput_Text, exception.Message),
                    Strings.FailedToParseInput_Caption,
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            foreach (var tag in _protobufMessage.Tags)
            {
                TreeNode node = null;

                if (tag is ProtobufTagSingle)
                {
                    node = new TreeNode($"Tag {tag.Index}") { Tag = tag, Name = tag.Index.ToString() };
                }
                else if (tag is ProtobufTagRepeated repeatedTag)
                {
                    node = new TreeNode($"Tag {tag.Index} (repeated)") { Tag = tag, Name = tag.Index.ToString() };

                    for (var index = 0; index < repeatedTag.Items.Length; index++)
                    {
                        var subTag = repeatedTag.Items[index];
                        node.Nodes.Add(
                            new TreeNode($"Instance {index + 1}")
                            {
                                Tag = subTag,
                                Name = subTag.Index + "-" + index
                            });
                    }
                }

                if (node != null)
                {
                    treeView1.Nodes.Add(node);
                }
            }

            PopulateByteViewer(input);
        }

        private void buttonClear_Click(object sender, EventArgs e)
        {
            propertyGridTag.SelectedObject = null;
            propertyGridTag.Update();
            treeView1.Nodes.Clear();
            dataGridViewBytes.DataSource = new List<ByteViewerRow>();
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            _isSelectingFromTreeView = true;

            try
            {
                ProtobufTag tag = null;

                if (treeView1.SelectedNode != null)
                {
                    tag = treeView1.SelectedNode.Tag as ProtobufTag;
                }

                propertyGridTag.SelectedObject = tag;
                propertyGridTag.Update();

                if (tag == null)
                {
                    // There is no selected node and therefore
                    // also no ProtobufTag to do anything with.
                    return;
                }

                var singleTag = tag as ProtobufTagSingle;

                if (singleTag == null)
                {
                    if (tag is ProtobufTagRepeated repeatedTag)
                    {
                        singleTag = repeatedTag.Items.First();
                    }
                }

                var startRow = singleTag.StartOffset / 16;
                var endRow = singleTag.EndOffset / 16;
                var startColumn = singleTag.StartOffset % 16; // n-th byte of a row
                var endColumn = singleTag.EndOffset % 16; // n-th byte of a row

                dataGridViewBytes.ClearSelection();

                for (var rowIndex = startRow; rowIndex <= endRow; rowIndex++)
                {
                    if (rowIndex == startRow)
                    {
                        // Start row
                        var end = startRow == endRow ? endColumn : 15;
                        for (var columnIndex = startColumn; columnIndex <= end; columnIndex++)
                        {
                            dataGridViewBytes[columnIndex + 1, rowIndex].Selected = true;
                        }
                    }
                    else if (
                        rowIndex == endRow &&
                        startRow !=
                        endRow) // When the tag fits on a single row this is already handled by the previous if-branch
                    {
                        // End row
                        for (var columnIndex = 0; columnIndex <= endColumn; columnIndex++)
                        {
                            dataGridViewBytes[columnIndex + 1, rowIndex].Selected = true;
                        }
                    }
                    else
                    {
                        // Middle row
                        for (var columnIndex = 0; columnIndex <= 15; columnIndex++)
                        {
                            dataGridViewBytes[columnIndex + 1, rowIndex].Selected = true;
                        }
                    }
                }

                // Bring selection into view
                dataGridViewBytes.FirstDisplayedScrollingRowIndex = startRow;
            }
            finally
            {
                _isSelectingFromTreeView = false;
            }
        }

        private void PopulateByteViewer(byte[] input)
        {
            var line = 0;
            var offset = 0;
            var rows = new List<ByteViewerRow>();

            var lineBytes = new byte[16];

            while (offset < input.Length)
            {
                var width = input.Length - offset;
                if (width >= 16)
                {
                    width = 16;
                }

                if (width >= 1) { lineBytes[0] = input[offset + 0]; } else { lineBytes[0] = 0; }
                if (width >= 2) { lineBytes[1] = input[offset + 1]; } else { lineBytes[1] = 0; }
                if (width >= 3) { lineBytes[2] = input[offset + 2]; } else { lineBytes[2] = 0; }
                if (width >= 4) { lineBytes[3] = input[offset + 3]; } else { lineBytes[3] = 0; }
                if (width >= 5) { lineBytes[4] = input[offset + 4]; } else { lineBytes[4] = 0; }
                if (width >= 6) { lineBytes[5] = input[offset + 5]; } else { lineBytes[5] = 0; }
                if (width >= 7) { lineBytes[6] = input[offset + 6]; } else { lineBytes[6] = 0; }
                if (width >= 8) { lineBytes[7] = input[offset + 7]; } else { lineBytes[7] = 0; }
                if (width >= 9) { lineBytes[8] = input[offset + 8]; } else { lineBytes[8] = 0; }
                if (width >= 10) { lineBytes[9] = input[offset + 9]; } else { lineBytes[9] = 0; }
                if (width >= 11) { lineBytes[10] = input[offset + 10]; } else { lineBytes[10] = 0; }
                if (width >= 12) { lineBytes[11] = input[offset + 11]; } else { lineBytes[11] = 0; }
                if (width >= 13) { lineBytes[12] = input[offset + 12]; } else { lineBytes[12] = 0; }
                if (width >= 14) { lineBytes[13] = input[offset + 13]; } else { lineBytes[13] = 0; }
                if (width >= 15) { lineBytes[14] = input[offset + 14]; } else { lineBytes[14] = 0; }
                if (width >= 16) { lineBytes[15] = input[offset + 15]; } else { lineBytes[15] = 0; }

                rows.Add(new ByteViewerRow(lineBytes, line));

                line++;
                offset += 16; // bytes per line
            }

            dataGridViewBytes.DataSource = rows;
        }

        private void dataGridViewBytes_SelectionChanged(object sender, EventArgs e)
        {
            if (_isSelectingFromTreeView)
            {
                return;
            }

            // Find the tag for the selected cell
            if (dataGridViewBytes.GetCellCount(DataGridViewElementStates.Selected) == 0)
            {
                return;
            }

            // Grab the first selected cell from the range that may have been selected
            var firstSelectedCell = dataGridViewBytes.SelectedCells[0];

            // Ignore selection of the row number cell
            if (firstSelectedCell.ColumnIndex == 0)
            {
                return;
            }

            var startColumn = firstSelectedCell.ColumnIndex - 1; // Take care of the row number column
            var startRow = firstSelectedCell.RowIndex * 16;
            var selectedOffset = startRow + startColumn;

            var tag = _protobufMessage
                .Tags
                .OfType<ProtobufTagSingle>() // This breaks repeated tags for now but hey..
                .FirstOrDefault(tag =>
                    tag.StartOffset <= selectedOffset &&
                    tag.EndOffset >= selectedOffset);

            TreeNode nodeToSelect = null;

            if (tag != null)
            {
                nodeToSelect = treeView1.Nodes.Find(tag.Index.ToString(), false).First();
            }

            if (treeView1.SelectedNode == nodeToSelect)
            {
                // We need to re-highlight on the DataGridView now
                treeView1_AfterSelect(treeView1, new TreeViewEventArgs(nodeToSelect, TreeViewAction.Unknown));
            }
            else
            {
                treeView1.SelectedNode = nodeToSelect;
            }
        }

        private void buttonDecodeTag_Click(object sender, EventArgs e)
        {
            if (treeView1.SelectedNode == null)
            {
                return;
            }

            var selectedNode = treeView1.SelectedNode;

            if (selectedNode.Tag == null)
            {
                return;
            }

            byte[] input = null;

            if (selectedNode.Tag is ProtobufTagSingle singleTag)
            {
                input = singleTag.Value.RawValue;
            }

            if (input != null)
            {
                var tagDecodeForm = new Main(input);

                tagDecodeForm.Show(this);
            }
        }
    }
}
