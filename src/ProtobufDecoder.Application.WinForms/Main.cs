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
                ShowMessageBox.ForNoInputSelected();

                return;
            }

            if (!File.Exists(inputFilePath))
            {
                ShowMessageBox.ForFileDoesNotExist();

                return;
            }

            byte[] input;

            try
            {
                input = File.ReadAllBytes(inputFilePath);
            }
            catch (IOException ioException)
            {
                ShowMessageBox.ForReadingInputFailed(ioException);

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
                ShowMessageBox.ForFailedToParseInput(exception);

                return;
            }

            var nodes = TreeNodeBuilder.BuildFromTags(_protobufMessage.Tags);

            foreach (var node in nodes)
            {
                treeView1.Nodes.Add(node);
            }

            dataGridViewBytes.DataSource = DataGridViewRowBuilder.Build(input);
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
                var tag = GetSelectedTag();

                propertyGridTag.SelectedObject = tag;

                var singleTag = tag as ProtobufTagSingle;

                if (singleTag == null)
                {
                    // Don't show anything for a repeated tag,
                    // only when an item for a repeated tag
                    // is selected.
                    return;
                }

                var byteViewOffset = GetOffsetOf(treeView1.SelectedNode);

                var startOffset = byteViewOffset + singleTag.StartOffset;
                var endOffset = byteViewOffset + singleTag.EndOffset;

                var startRow = startOffset / 16;
                var endRow = endOffset / 16;

                // Handle case where EndOffset is a multiple of 16
                // which causes an off-by-one error for the row index.
                if (endOffset % 16 == 0)
                {
                    endRow -= 1;
                }

                var startColumn = startOffset % 16; // n-th byte of a row
                var endColumn = endOffset % 16; // n-th byte of a row

                // Handle the case where the endColumn is exactly
                // divisible by 16 which would cause byte 32 for
                // example to highlight to column 0...
                if (endColumn == 0)
                {
                    endColumn = 15;
                }

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

        private static int GetOffsetOf(TreeNode treeNode, int offset = 0)
        {
            if (treeNode.Parent == null)
            {
                return 0;
            }

            treeNode = treeNode.Parent;

            if (treeNode.Tag is ProtobufTagSingle singleTag)
            {
                // Add the StartOffset of this tag
                return singleTag.StartOffset + GetOffsetOf(treeNode, offset);
            }

            if (treeNode.Tag is ProtobufTagRepeated)
            {
                // Don't add any additional offset because
                // a repeated tag is only a placeholder
                return offset + GetOffsetOf(treeNode, offset);
            }

            return offset;
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

        private void DecodeTagInWindowCommand(object sender, EventArgs e)
        {
            var tag = GetSelectedTag();
            
            if (!(tag is ProtobufTagSingle singleTag && singleTag.Value.CanDecode))
            {
                ShowMessageBox.ForTagDecodingNotSupported();

                return;
            }

            var input = GetRawBytesOfSelectedTag(tag);

            if (input == null)
            {
                return;
            }

            try
            {
                var tagDecodeForm = new Main(input);

                tagDecodeForm.Show(this);
            }
            catch (Exception exception)
            {
                ShowMessageBox.ForFailedToDecodeTag(exception);
            }
        }

        private void DecodeTagInPlaceCommand(object sender, EventArgs e)
        {
            var tag = GetSelectedTag();
            
            if (!(tag is ProtobufTagSingle singleTag && singleTag.Value.CanDecode))
            {
                ShowMessageBox.ForTagDecodingNotSupported();

                return;
            }

            var input = GetRawBytesOfSelectedTag(tag);

            if (input == null)
            {
                return;
            }

            try
            {
                var parsedMessage = ProtobufParser.Parse(input);

                var nodes = TreeNodeBuilder.BuildFromTags(parsedMessage.Tags);

                foreach (var node in nodes)
                {
                    treeView1.SelectedNode.Nodes.Add(node);
                }
                    
                treeView1.SelectedNode.Expand();
            }
            catch (Exception exception)
            {
                ShowMessageBox.ForFailedToDecodeTag(exception);
            }
        }

        private static byte[] GetRawBytesOfSelectedTag(ProtobufTag tag)
        {
            byte[] input = null;

            if (tag is ProtobufTagSingle singleTag)
            {
                input = singleTag.Value.RawValue;
            }

            return input;
        }

        private ProtobufTag GetSelectedTag()
        {
            if (treeView1.SelectedNode == null)
            {
                return null;
            }

            var selectedNode = treeView1.SelectedNode;

            return selectedNode.Tag as ProtobufTag;
        }
    }
}
