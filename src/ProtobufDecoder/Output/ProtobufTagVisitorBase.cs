using System.Text;
using ProtobufDecoder.Tags;

namespace ProtobufDecoder.Output
{
    public abstract class ProtobufTagVisitorBase
    {
        protected StringBuilder Builder { get; }

        protected ProtobufTagVisitorBase(StringBuilder builder)
        {
            Builder = builder;
        }

        public void Visit(ProtobufTag tag)
        {
            // Because of inheritance of tag types the order
            // here is important.

            if (tag is ProtobufTagEmbeddedMessage embedded)
            {
                Visit(embedded);
            }
            else if (tag is ProtobufTagPacked packed)
            {
                Visit(packed);
            }
            else if (tag is ProtobufTagString stringTag)
            {
                Visit(stringTag);
            }
            else if (tag is ProtobufTagLengthDelimited lengthDelimited)
            {
                Visit(lengthDelimited);
            }
            else if(tag is ProtobufTagSingle singleTag)
            {
                Visit(singleTag);
            }
            else if (tag is ProtobufTagRepeated repeated)
            {
                Visit(repeated);
            }
        }

        protected abstract void Visit(ProtobufTagSingle tag);

        protected abstract void Visit(ProtobufTagRepeated tag);

        protected abstract void Visit(ProtobufTagEmbeddedMessage tag);

        protected abstract void Visit(ProtobufTagPacked tag);

        protected abstract void Visit(ProtobufTagString tag);
    }
}