using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ProtobufDecoder.Tags
{
    /// <summary>
    /// A virtual tag that contains all occurences of a tag number
    /// </summary>
    /// <remarks>Protobuf allows a tag number to appear multiple times, this type acts as a collection of those types and isn't a "real" tag itself</remarks>
    public class ProtobufTagRepeated : ProtobufTag
    {
        [Browsable(false)]
        public ObservableCollection<ProtobufTagSingle> Items { get; set; } = new ObservableCollection<ProtobufTagSingle>();

        /// <summary>
        /// <c>true</c> when  all items are embedded messages
        /// </summary>
        [Browsable(false)]
        public bool ContainsOnlyEmbeddedMessages => Items.Any() && Items.All(item => item is ProtobufTagEmbeddedMessage);
    }
}