using System.Collections.Generic;
using System.Windows.Forms;

namespace ProtobufDecoder.Application.WinForms
{
    static internal class TreeNodeBuilder
    {
        public static List<TreeNode> BuildFromTags(List<ProtobufTag> tags)
        {
            var nodes = new List<TreeNode>();

            foreach (var tag in tags)
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
                    nodes.Add(node);
                }
            }

            return nodes;
        }
    }
}