using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Application.Wpf.ViewModels
{
    public interface IProtobufParent
    {
        bool IsSelected { get; set; }
        bool IsExpanded { get; set; }
        IProtobufParent Parent { get; set; }

        void ReplaceChildWith(
            ProtobufTagSingle child,
            ProtobufTagSingle replacement);
    }
}